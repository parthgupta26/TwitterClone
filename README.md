# COP5615 Distributed Operating System Principles Project 3

## Chord-P2P-System-And-Simulation-Actor-Model-F#-Akka.Net

## Project Description

You have to implement the network join and routing as described in the Chord paper (Section 4) and encode the simple application that associates a key (same as the ids used in Chord) with a string. You can change the message type sent and the specific activity as long as you implement it using a similar API to the one described in the paper. <br>
Link: https://pdos.csail.mit.edu/papers/ton:chord/paper-ton.pdf

## Project Requirements

### Input

The input provided (as command line) will be of the form: numberOfNodes numberOfRequests Where numberOfNodes is the number of peers to be created in the peer-to-peer system and numberOfRequests is the number of requests each peer has to make. When all peers performed that many requests, the program can exit. Each peer should send a request/second.

### Actor modeling

In this project, you have to use exclusively the AKKA actor framework (projects that do not use multiple actors or use any other form of parallelism will receive no credit).  You should have one actor for each of the peers modeled.

### Output

Print the average number of hops (node connections) that have to be traversed to deliver a message.

### Example:


```F#
dotnet fsi Project3.fsx 2500 100
```
```F#
Average number of hops (node connections) that have to be traversed to deliver a message : 5.295244
```

## Submitted By:

Name: Parth Gupta, UFID: 91997064

## What is Working?

- Implemented the network join and routing as described in the Chord paper and encoded the application that associates a key with a string.
- After implementation of Chord Algorithm, I am printing the average hop count that has to be traversed to deliver a message.

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
