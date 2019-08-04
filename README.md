TODO:

1. Hide the token
2. Set up vm to run this in cloud .. running atm on raspi but lavalink doesn't like it too much 
    - make publish version from this 
     - How to deal with updates etc?

3. More commands and add some comments
4. Test cases!
5. 


##Buglist

1. If the bot is too long paused and you try to use it after long break etc. 20min paused
    it crashes or the.. so solution is auto timeout after 5min when not used or paused or something like it
		- Maybe make listener for StopAsync method and make function that will disconnect bot if not used by users..

2. 