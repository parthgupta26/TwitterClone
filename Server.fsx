#r "nuget: Akka" 
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.Remote" 
#r "nuget: Akka.TestKit" 

open Akka.Actor
open Akka.Configuration
open Akka.FSharp

let configuration = 
    ConfigurationFactory.ParseString(
        @"akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                debug : {
                    receive : on
                    autoreceive : on
                    lifecycle : on
                    event-stream : on
                    unhandled : on
                }
            }
            remote {
                helios.tcp {
                    port = 5000
                    hostname = localhost
                }
            }
        }")

[<Struct>]         
type guid = {
    id : string
}
type registerMsg = {
    code : int; 
    userId : guid; 
    password : string
}
type tweetMsg = {
    userId : guid; 
    tweet : string
}
type subscribeMsg = {
    userId : guid; 
    subscriberId : guid
}
type hashTagMsg = {
    ht : string; 
    userId : guid
}
type mentionMsg = {
    men : string; 
    userId : guid
}
type retweetMsg = {
    userId : guid; 
    tweet : string
}

let mutable userIdSet: Set<string> = Set.empty;
let mutable userId:  Map<string, string> = Map.empty;
let mutable onlineUsers:  Map<string, bool> = Map.empty;
let mutable hashTags: Map<string, list<string>> = Map.empty;
let mutable mentionUsers: Map<string, list<string>> = Map.empty;
let mutable pendingRequests: Map<string, list<string>> = Map.empty;
let mutable followers: Map<string, list<string>> = Map.empty;
let mutable followingMap: Map<string, list<string>> = Map.empty;

let system = System.create "Twitter" configuration

let registerUser (id: string) (password: string) =
    let mutable found = "";
    if (userIdSet.Contains id) then
        found <- "user already exists";
    else 
        userIdSet <- userIdSet.Add(id);
        userId <- userId.Add(id, password);
        onlineUsers <- onlineUsers.Add(id, true);
        mentionUsers <- mentionUsers.Add(id, []);
        followingMap <- followingMap.Add(id, List.empty);
        followers <- followers.Add(id, List.empty);
        pendingRequests <- pendingRequests.Add(id, []);
        found <- "registration is successful";
    found

let ActivateUser (userId: string) (newStatus: bool) =
    onlineUsers <- onlineUsers.Add(userId, newStatus)

let addUserAsFollowing (user: string) (following: string) =
    let mutable followingUser = followingMap.Item user;
    followingUser <- followingUser @ [ following ];
    followingMap <- followingMap.Add(user, followingUser);

let addFollowerAndLeader (userId: string) (leaderId: string) =
    let mutable found = false;
    if((userIdSet.Contains(userId)) && (userIdSet.Contains(leaderId))) then
        addUserAsFollowing userId leaderId
        addUserAsFollowing userId leaderId
        found <- true;
    found


let retreiveFromList (arr: string list) (symbol: char) =
    let mutable dataList : List<string> = List.Empty;
    let myVar = symbol.ToString();
    for i in arr do
        if i.StartsWith myVar then
            dataList <- List.append dataList [i];
    dataList

let getHashTags (arr: string list) =   
    retreiveFromList arr '#'

let getMentions (arr: string list) =
    retreiveFromList arr '@'

let saveHashTags (hashtagList: list<string>) (tweet: string) =
    for hashtag in hashtagList do
        if hashTags.ContainsKey hashtag then
            let currentTweetList = hashTags.Item hashtag
            let tweetList = currentTweetList @ [ tweet ]
            hashTags <- hashTags.Add(hashtag, tweetList)
        else
            let tweetList = [ tweet ]
            hashTags <- hashTags.Add(hashtag, tweetList)


let pushFeedToUser (username: string) =
    let pendingFeed = pendingRequests.Item username
    let newEmptyFeed = [ "start" ]
    pendingRequests <- pendingRequests.Add(username, newEmptyFeed)
    pendingFeed

type Message_reg = MessageReg of  int  * guid  * string * IActorRef
type Message_tweet = MessageTweet of  guid * string * IActorRef
type Message_retweet = MessageRetweet of  guid * string * IActorRef
type Message_subscribe = MessageSubscribe of  guid * guid * IActorRef
type Message_HT = MessageHT of  string * IActorRef
type Message_Mention = MessageMention of  string * IActorRef

let MentionWorker (mailbox: Actor<_>)=
        let rec loop() =
            actor {
                let! message = mailbox.Receive()
                match message with
                | MessageMention(mentionedUser,sender) -> 
                        if (mentionUsers.ContainsKey mentionedUser) then
                            let mentionedList = mentionUsers.Item mentionedUser
                            let mutable res = "";
                            for men in mentionedList do
                                res <- res + "***" + men;
                            sender <! res
                        else
                            sender <! "no such mentions"
                return! loop()
            } 
        loop()

let HashTagWorker (mailbox: Actor<_>)=
    let rec loop() =
        actor {
            let! message = mailbox.Receive()
            match message with
            | MessageHT(hashtag,sender) -> 
                    if (hashTags.ContainsKey hashtag) then
                        let hashtagTweets = hashTags.Item hashtag
                        let mutable res = "";
                        for ht in hashtagTweets do
                            res <- res + "***" + ht;
                        sender <! res
                    else
                        sender <! "no such hashtags"
            return! loop()
        } 
    loop()

let SubscribeWorker (mailbox: Actor<_>)=
    let rec loop() =
        actor {
            let! message = mailbox.Receive()

            match message with
            | MessageSubscribe(user, peer, sender) -> 
                    addFollowerAndLeader user.id peer.id |> ignore
                    sender <! "success"
            return! loop()
        } 
    loop()

let ReweetsWorker (mailbox: Actor<_>)=
    let rec loop() =
        actor {
            let! message = mailbox.Receive()

            match message with
            | MessageRetweet(user, tweet, sender) -> 
                    let followersList = followers.Item user.id
                    for follower in followersList do
                        if(tweet.StartsWith "Retweetedby") then
                            let arr = tweet.Split('|') |> Array.toList
                            let currentToUserFeed = pendingRequests.Item follower
                            let hybridTweet = "Retweetedby:" + user.id + "|" + arr.[1]
                            let newToUserFeed = currentToUserFeed @ [ hybridTweet ]
                            pendingRequests <- pendingRequests.Add(follower, newToUserFeed)
                        else
                            let currentToUserFeed = pendingRequests.Item follower
                            let hybridTweet = "Retweetedby:" + user.id + "|" + tweet
                            let newToUserFeed = currentToUserFeed @ [ hybridTweet ]
                            pendingRequests <- pendingRequests.Add(follower, newToUserFeed)

                    sender <! "success"
            return! loop()
        } 
    loop()

let TweetsWorker (mailbox: Actor<_>)=
    let rec loop() =
        actor {
            let! message = mailbox.Receive()

            match message with
            | MessageTweet(user, tweet, sender) -> 
                    let hashTags = getHashTags (tweet.Split(' ') |> Array.toList)
                    saveHashTags hashTags tweet
                    let mentions = getMentions (tweet.Split(' ') |> Array.toList)
                    for mention in mentions do
                        if mentionUsers.ContainsKey mention then
                            let currentTweetList = mentionUsers.Item mention
                            let tweetList = currentTweetList @ [ tweet ]
                            mentionUsers <- mentionUsers.Add(mention, tweetList)
                        else
                            let tweetList = [ tweet ]
                            mentionUsers <- mentionUsers.Add(mention, tweetList)
                    let followersList = followers.Item user.id
                    for follower in followersList do
                        let currentToUserFeed = pendingRequests.Item follower
                        let hybridTweet = "author:" + user.id + " = " + tweet
                        let newItemInUserFeed = currentToUserFeed @ [ hybridTweet ]
                        pendingRequests <- pendingRequests.Add(follower, newItemInUserFeed)

                    sender <! "success"
            return! loop()
        } 
    loop()

let AuthenticationWorker (mailbox: Actor<_>)=
    let rec loop() =
        actor {
            let! message = mailbox.Receive()

            match message with
            | MessageReg(code, username, pwd, sender) -> 
                if(code = 0) then
                    let res = registerUser username.id pwd
                    sender <! res
                else if(code = 1) then
                    let mutable res = "login failed";
                    if(userId.ContainsKey username.id) then
                        let pword = userId.Item username.id
                        if (pword = pwd) then
                            ActivateUser username.id true
                            res <- "";
                            let pfeed = pushFeedToUser username.id
                            for pf in pfeed do
                                res <- res + "***" + pf;
                        sender <! res
                else
                    ActivateUser username.id false
                    sender <! "successfully logged out"
            return! loop()
        } 
    loop()

let AuthWorker = spawn system "AuthenticationWorker" AuthenticationWorker
let TweetWorker = spawn system "TweetWorker" TweetsWorker
let RetweetWorker = spawn system "RetweetWorker" ReweetsWorker
let SubscriberWorker = spawn system "SubscriberWorker" SubscribeWorker
let HashTagsWorker =  spawn system "HashTagsWorker" HashTagWorker
let MentionsWorker =  spawn system "MentionsWorker" MentionWorker

let isUserLoggedIn (userId: guid) =
    let mutable found = false;
    if(onlineUsers.ContainsKey userId.id) then
        if(onlineUsers.Item userId.id) then
            found <- true;
    found

let echoServer = 
    spawn system "EchoServer"
    <| fun mailbox ->
        let rec loop() =
            actor {
                let! message = mailbox.Receive()
                let sender = mailbox.Sender()
                let userMessage = "user not logged in"
                
                match box message with
                | :? registerMsg as msg-> 
                    AuthWorker <! MessageReg(msg.code, msg.userId, msg.password, sender)

                | :? tweetMsg as msg-> 
                    if (isUserLoggedIn msg.userId) then
                        TweetWorker <! MessageTweet(msg.userId, msg.tweet, sender)
                    else
                        sender <! userMessage

                | :? retweetMsg as msg-> 
                    if (isUserLoggedIn msg.userId) then
                        RetweetWorker <! MessageRetweet(msg.userId, msg.tweet, sender)
                    else
                        sender <! userMessage

                | :? subscribeMsg as msg-> 
                    if (isUserLoggedIn msg.userId) then
                        SubscriberWorker <! MessageSubscribe(msg.userId, msg.subscriberId, sender)
                    else
                        sender <! userMessage

                | :? hashTagMsg as msg-> 
                    if (isUserLoggedIn msg.userId) then
                        HashTagsWorker <! MessageHT(msg.ht, sender)
                    else
                        sender <! userMessage

                | :? mentionMsg as msg-> 
                    if (isUserLoggedIn msg.userId) then
                        MentionsWorker <! MessageMention(msg.men, sender)
                    else
                        sender <! userMessage

                | _ ->  failwith "Unknown message"
                
                return! loop()
            }
        loop()

System.Console.ReadLine() |> ignore
system.Terminate() |> ignore