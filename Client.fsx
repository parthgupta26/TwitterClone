#r "nuget: Akka" 
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.Remote" 
#r "nuget: Akka.TestKit" 

open Akka.Actor
open Akka.Configuration
open Akka.FSharp

let args = fsi.CommandLineArgs
let numberOfUsers = args.[1] |> int

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
                    port = 8123
                    hostname = localhost
                }
            }
        }")

let system = ActorSystem.Create("Twitter", configuration)
let echoClient = system.ActorSelection("akka.tcp://Twitter@localhost:5000/user/EchoServer")

let mutable count = 0;

type NodeMessage =
    |Start of (int)
    |Inp
    |End of (int)

let hts = [|"#ht1" ; "#ht2" ; "#ht3" ; "#ht4" ; "#ht5" ; "#ht6"|]

let Node (listener : IActorRef) (x : int) (mailBox:Actor<_>) = 
        let rec loop() = actor {
            
            let! message = mailBox.Receive()
                    
            match message with 
                | Start(x) ->
                        let mutable responses = "";
                        let userId = ((new System.Random()).Next(numberOfUsers)) + 1
                        // printfn "%d  --" userId
                        let j = ((new System.Random()).Next(6)) 
                        if (j = 0) then
                            let tweetID = {guid.id = "user" + userId.ToString()}
                            let tweet = "tweet" + userId.ToString() + " #game @user1"
                            let task = echoClient <? {tweetMsg.userId = tweetID; tweetMsg.tweet = tweet}
                            responses <- Async.RunSynchronously (task, 3000);
                        else if (j = 1) then
                            let registerMessageID = {guid.id = "user" + userId.ToString()}
                            let registerMessagePassword = "password" + userId.ToString()
                            let task = echoClient <? {registerMsg.code = 2; registerMsg.userId = registerMessageID; registerMsg.password = registerMessagePassword}
                            responses <- Async.RunSynchronously (task, 1000);
                        else if (j = 2) then
                            let registerMessageID = {guid.id = "user" + userId.ToString()}
                            let registerMessagePassword = "password" + userId.ToString()
                            let task = echoClient <? {registerMsg.code = 1; registerMsg.userId = registerMessageID; registerMsg.password = registerMessagePassword}
                            responses <- Async.RunSynchronously (task, 1000);
                        else if (j = 3) then
                            let l1 = ((new System.Random()).Next(6))
                            let hashtagMessage = {guid.id = "user" + userId.ToString()}
                            let task = echoClient <? {hashTagMsg.ht = hts.[l1]; hashTagMsg.userId = hashtagMessage}
                            responses <- Async.RunSynchronously (task, 1000);
                        else if (j = 4) then
                            let mentionMessage = "@user" + userId.ToString()
                            let mentionUserID = {guid.id = "user" + userId.ToString()}
                            let task = echoClient <? {mentionMsg.men = mentionMessage; mentionMsg.userId = mentionUserID}
                            responses <- Async.RunSynchronously (task, 1000);
                        else if (j = 5) then
                            let retweetUserID = {guid.id = "user2"}
                            let retweetUserMessage = "Retweetedby:usery|author:userx = tweetx #game @user1"
                            let task = echoClient <? {retweetMsg.userId = retweetUserID; retweetMsg.tweet = retweetUserMessage}
                            responses <- Async.RunSynchronously (task, 1000);
                        listener <! End(userId)

                | _-> return! loop()
            return! loop()
        }
        loop()

let BossActor (mailbox:Actor<_>) =
    let rec loop () = actor {
        let! message = mailbox.Receive()
        match message with 
        | Inp ->
            let node = Node mailbox.Context.Self (0) |> spawn system ("Node" + id.ToString())
            for i in 1 .. numberOfUsers do
                node <! Start(i)

        | End(x) ->
            count <- count + 1;
            if (count = numberOfUsers) then
                mailbox.Context.System.Terminate() |> ignore

        | _ -> ()
        return! loop()
    }
    loop()

let mutable response = "";

let mutable stopWatch = System.Diagnostics.Stopwatch.StartNew();

let mutable i = 1;
while i < numberOfUsers do
    // printfn "%d" i
    let messageUserID = {guid.id = "user" + i.ToString()}
    let messagePassword = "password" + i.ToString()
    let task = echoClient <? {registerMsg.code = 0; registerMsg.userId = messageUserID; registerMsg.password = messagePassword}
    response <- Async.RunSynchronously (task, 1000);
    i <- i + 1;

stopWatch.Stop();
let timeToRegisterUsers = stopWatch.Elapsed.TotalMilliseconds

stopWatch <- System.Diagnostics.Stopwatch.StartNew();

let mutable step = 1;
for i in 1..numberOfUsers do
    for j in 1..step..numberOfUsers do
        // printfn "%d == %d" i j
        let mutable res = -1;
        if ( i <> j ) then
            let a =
                if (j <> 0) then
                    res <- j;
                else
                    res <- i / 2;
                res
            let subscribingUserID = {guid.id = "user" + a.ToString()}
            let subscribingSubscriberID = {guid.id = "user" + i.ToString()}
            let task = echoClient <? {subscribeMsg.userId = subscribingUserID; subscribeMsg.subscriberId = subscribingSubscriberID}
            response <- Async.RunSynchronously (task, 1000);
    step <- (i + 1) * (i + 1);

// printfn "Done Subscribing"
stopWatch.Stop();
let timeToSubscribe = stopWatch.Elapsed.TotalMilliseconds

stopWatch <- System.Diagnostics.Stopwatch.StartNew();

let mutable k = 1;
while k < numberOfUsers do 
    let j = ((new System.Random()).Next(5))
    let tweetMessageID = {guid.id = "user" + k.ToString()}
    let tweetMessage = "tweet" + k.ToString() + "0" + " " + hts.[j] + " @user" + (k / 2).ToString()
    let task = echoClient <? {tweetMsg.userId = tweetMessageID; tweetMsg.tweet =  tweetMessage}
    response <- Async.RunSynchronously (task, 1000);
    k <- k + 1;

stopWatch.Stop();
let timeToSendTweets = stopWatch.Elapsed.TotalMilliseconds

stopWatch <- System.Diagnostics.Stopwatch.StartNew();

let boss = spawn system "boss" BossActor
boss <? Inp
system.WhenTerminated.Wait()

stopWatch.Stop();
let timeToPerformOperations = stopWatch.Elapsed.TotalMilliseconds

printfn "**********************************************************************" 
printfn "The time taken to register %d users is %f" numberOfUsers timeToRegisterUsers
printfn "The time taken to subscribe in Zipf distance for %d users is %f" numberOfUsers timeToSubscribe
printfn "The time taken to send tweets for %d users is %f" numberOfUsers timeToSendTweets
printfn "The time taken to perform random operations for %d users is %f" numberOfUsers timeToPerformOperations
printfn "**********************************************************************"

system.Terminate() |> ignore