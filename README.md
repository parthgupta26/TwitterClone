# COP5615 Distributed Operating System Principles Project 4 Part1

## Twitter-Clone-Actor-Model-F#-Akka.Net

## Project Description

In this project, you have to implement a Twitter Clone and a client tester/simulator. As of now, Tweeter does not seem to support a WebSocket API. As part I of this project, you need to build an engine that (in part II) will be paired up with WebSockets to provide full functionality. Specific things you have to do are: 

### Implement a Twitter-like engine with the following functionality:

- Register account
- Send tweet. Tweets can have hashtags (e.g. #COP5615isgreat) and mentions (@bestuser).
- Subscribe to user's tweets.
- Re-tweets (so that your subscribers get an interesting tweet you got by other means).
- Allow querying tweets subscribed to, tweets with specific hashtags, tweets in which the user is mentioned (my mentions).
- If the user is connected, deliver the above types of tweets live (without querying).

### Implement a tester/simulator to test the above

- Simulate as many users as you can.
- Simulate periods of live connection and disconnection for users.
- Simulate a Zipf distribution on the number of subscribers. For accounts with a lot of subscribers, increase the number of tweets. Make - some of these messages re-tweets.

### Other considerations:

- The client part (send/receive tweets) and the engine (distribute tweets) have to be in separate processes. Preferably, you use multiple independent client processes that simulate thousands of clients and a single-engine process.
- You need to measure various aspects of your simulator and report performance.
- More detail in the lecture as the project progresses.

### Output

```F#
dotnet fsi Server.fsx
dotnet fsi Client.fsx 1000
```
```F#
lhost%3A5000-1] Removing receive buffers for [akka.tcp://Twitter@localhost:8123]->[akka.tcp://Twitter@localhost:5000]
[INFO][30-11-2021 22:49:45][Thread 0008][remoting (akka://Twitter)] Remoting shut down
[INFO][30-11-2021 22:49:45][Thread 0022][remoting-terminator] Remoting shut 
down.
**********************************************************************      
The time taken to register 1000 users is 1227.643000
The time taken to subscribe in Zipf distance for 1000 users is 1915.750200  
The time taken to send tweets for 1000 users is 571.997200
The time taken to perform random operations for 1000 users is 792.758100    
**********************************************************************      
PS C:\Users\Parth Gupta\desktop\Project 4>
```

## Submitted By:

Name: Parth Gupta, UFID: 91997064

## Some Results of my Algorithm:

| Number of Clients | Time taken to register a user | Time taken to subscribe in Zipf Dist. | Time taken to tweet initially | Time taken to perform random operations (tweet, retweet, etc) |
| --------------- | ------------------ | ------------------ | ------------------ | ------------------ |
| 50 | 489.5254 | 250.1754 | 98.3377 | 231.2165 |
| 100 |	587.0261 | 460.6575 | 206.3094 | 328.9131 |
| 250 |	853.2033 | 1165.2397 | 376.7394 | 570.2716 |
| 500 |	1203.275 | 2117.5105 |	671.0764 | 851.67 |
| 1000 | 2027.2003 | 4146.9993 | 1342.1305 | 1549.8689 |
| 2000 | 3389.5079 | 7432.5802 | 2677.1804 | 3632.117 |
| 2500 | 3902.7208 | 9385.075 |	3336.1824 |	5242.0079 |
| 4000 | 5712.6045 | 15270.2066 | 5325.4921 | 10902.7856 |
| 5000 | 6874.4138 | 19400.725 | 6619.649 |	18237.8286 |
| 6000 | 8707.9989 | 24621.5714 | 7891.9751 | 25081.1 |
| 7500 | 7789.3301 | 29140.3329 | 10264.2001 | 43301.1474 |
| 10000 | 13926.2079 | 43512.213 | 13806.7026 |	113794.8561 |
| 12500 | 18419.7653 | 46886.4428 |	15108.748 |	196039.7073 |
| 15000 | 16300.514 | 58712.6919 | 15378.415 | 289675.22 |

## The largest network that I managed to deal with:

For this project, the largest network that I was able to manage was for 15000 users.

```F#
[INFO][30-11-2021 22:49:45][Thread 0008][remoting (akka://Twitter)] Remoting shut down
[INFO][30-11-2021 22:49:45][Thread 0022][remoting-terminator] Remoting shut 
down.
**********************************************************************      
The time taken to register 15000 users is 16300.514000
The time taken to subscribe in Zipf distance for 15000 users is 58712.691900 
The time taken to send tweets for 15000 users is 15378.415000
The time taken to perform random operations for 15000 users is 289675.220000  
**********************************************************************      
PS C:\Users\Parth Gupta\desktop\Project 4>
```

## Built On

- Programming language: F# 
- Framework: AKKA.NET
- Operating System: Windows 10
- Programming Tool: Visual Studio Code
