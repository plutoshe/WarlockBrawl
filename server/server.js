var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

server.listen(3000);

// global variables for the server
var enemies = [];
var playerSpawnPoints = [];
var clients = [];

app.get('/', function(req, res) {
    res.send('hey you got back get "/"');
});

io.on('connection', function(socket) {

    var currentPlayer = {};
    currentPlayer.name = 'unknown';

    socket.on('player connect', function() {
        console.log(currentPlayer.name+' recv: player connect');
        for(var i =0; i<clients.length;i++) {
            var playerConnected = {
                name:clients[i].name,
                position:clients[i].position,
                rotation:clients[i].position,
                health:clients[i].health
            };
            // in your current game, we need to tell you about the other players.
            socket.emit('other player connected', playerConnected);
            console.log(currentPlayer.name+' emit: other player connected: '+JSON.stringify(playerConnected));
        }
    });

    socket.on('play', function(data) {
        console.log(currentPlayer.name+' recv: play: '+JSON.stringify(data));
        // if this is the first person to join the game init the enemies
        if(clients.length === 0) {
            numberOfEnemies = data.enemySpawnPoints.length;
            enemies = [];
            data.enemySpawnPoints.forEach(function(enemySpawnPoint) {
                var enemy = {
                    name: guid(),
                    position: enemySpawnPoint.position,
                    rotation: enemySpawnPoint.rotation,
                    health: 100
                };
                enemies.push(enemy);
            });
            playerSpawnPoints = [];
            data.playerSpawnPoints.forEach(function(_playerSpawnPoint) {
                var playerSpawnPoint = {
                    position: _playerSpawnPoint.position,
                    rotation: _playerSpawnPoint.rotation
                };
                playerSpawnPoints.push(playerSpawnPoint);
            });
        }

        var enemiesResponse = {
            enemies: enemies
        };
        // we always will send the enemies when the player joins
        console.log(currentPlayer.name+' emit: enemies: '+JSON.stringify(enemiesResponse));
        socket.emit('enemies', enemiesResponse);
        var randomSpawnPoint = playerSpawnPoints[Math.floor(Math.random() * playerSpawnPoints.length)];
        currentPlayer = {
            name:data.name,
            position: randomSpawnPoint.position,
            rotation: randomSpawnPoint.rotation,
            health: 100
        };
        clients.push(currentPlayer);
        // in your current game, tell you that you have joined
        console.log(currentPlayer.name+' emit: play: '+JSON.stringify(currentPlayer));
        socket.emit('play', currentPlayer);
        // in your current game, we need to tell the other players about you.
        socket.broadcast.emit('other player connected', currentPlayer);
    });
});