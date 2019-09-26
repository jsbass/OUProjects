var Game = function (container) {
	var div = null
	var canvas = document.createElement("canvas");
	if(typeof container === "undefined"){
		div = document.body;
		
	} else {
		div = container;
	}
	div.appendChild(canvas);
	
	console.log(this.h);
	var that = this;
	var scale = 1;
	this.requestId = -1;
	
	this.ctx = canvas.getContext("2d");
	var resize = function(first) {
		var min = Math.min(div.clientHeight, div.clientWidth);
		canvas.width = min;
		canvas.height = min;
		scale = min/1000;
	}
	var w0 = 0;
	var h0 = 0;
	setInterval(function(){
		if(div.clientHeight != h0 || div.clientWidth != w0){
			resize();
		}
	}, 100);
	var reset = function() {
		console.log("reset");
		that.time = 0;
		that.score = 0;
		that.bullets = [];
		that.asteroids = [];
		that.ship = new Ship(500, 500, 2000, 2500, 5);
		fireReset = 0;
		spawnReset = 0;
	}
	reset();
	resize(true);
	var debug = {
		dt: 0,
		previousState: -1
	};
	this.keysPressed = {};
	window.onkeyup = function(e) {that.keysPressed[e.code] = false; e.preventDefault(); };
	window.onkeydown = function(e) {that.keysPressed[e.code] = true; e.preventDefault(); };
	
	this.bulletSpeed = this.ship.maxSpeed;
	this.bulletLife = this.w/this.bulletSpeed;
	this.bulletR = 2;
	
	this.asteroidSpeed = 10;
	
	this.damping = .5;
	
	var last = null;
	//states:
	//0: paused
	//1: playing
	//2: gameover
	//-1: debug
	var state = 0;
	
	var getRandomPos = function(h,w) {
		var r = Math.floor(random() * 80) ///Random number between 0 and 80;
		if(r >= 40) r += 20;
		return r;
	}
	
	that.debug = function () {
		if(that.state === -1){
			that.state = debug.previousState;
		} else {
			console.log("debug");
			debug.previousState = that.state;
			that.state = -1;
		}
	}
	this.pause = function() {
		pause();
	}
	
	this.play = function () {
		play();
		that.requestId = requestAnimationFrame(loop);
	}
	
	var pause = function() {
		that.state = 0;
	}
	var play = function() {
		if(that.state === 2) {
			reset();
		}
		that.state = 1;
	}
	
	var gameOver = function() {
		//stop loop
		that.state = 2;
	}
	
	var loop = function(time){
		if(!last) last = time;
		var dt = time - last;
		last = time;
		
		debug.dt = dt;
		debug.time += dt;
		switch (that.state){
			case 0: //paused
				dt = 0;
				break;
			case 1: //playing
				break;
			case 2: //game over
				dt = 0;
				break;
			case -1: //debug
				break;
		}
		
		update(dt/1000);
		render();
		that.requestId = requestAnimationFrame(loop);
	}
	
	this.fireRate = 3; //shots per second
	var fireReset = 0;
	
	this.startSpawnRate = .2
	//this.endSpawnRate = 5;
	this.spawnRate = this.startSpawnRate; //large asteroids per second
	var spawnReset = 0;
	
	this.spawnAsteroid = function(size) {
		var rot = Math.random() * 270;
		if(rot < 45) rot += 45;
		if(rot > 315) rot -= 315;
		var r = Math.random() * 500 + 250;
		var x = that.ship.x + r*Math.cos(rot);
		var y = that.ship.y + r*Math.sin(rot);
		that.asteroids[that.asteroids.length] = new Asteroid(size, x, y);
	}
	
	var update = function (dt) {
		switch(that.state){
			case 0: //paused
				if(that.keysPressed["Escape"] && that.keyup){
					that.keyup = false;
					play();
				} else if(!that.keysPressed["Escape"]){
					that.keyup = true;
				}
				break;
			case 1: //playing
				that.time += dt;
				if(that.keysPressed["Escape"] && that.keyup){
					that.keyup = false;
					pause();
				} else if(!that.keysPressed["Escape"]) {
					that.keyup = true;
				}
				if(that.keysPressed["KeyP"]){
					that.debug();
				}
				//increment the fire reset timer
				if(fireReset > 0){
					fireReset -= dt;
				}
				
				if(spawnReset > 0){
					spawnReset -= dt;
				} else {
					that.spawnAsteroid(3);
					console.log(that.asteroids.length);
					spawnReset = 1/that.spawnRate;
				}
				
				if(that.keysPressed["Space"] && fireReset <= 0) {
					console.log("fire");
					fireReset = 1/that.fireRate;
					//var coords = rotateRef(that.ship.x, that.ship.y + that.ship.h/2, that.ship.rot, that.ship.x, that.ship.y);
					var coords = {x: that.ship.x, y: that.ship.y};
					that.bullets[that.bullets.length] = new Bullet(that.bulletR, coords.x, coords.y, that.bulletSpeed, that.ship.rot);
				}
				
				//Check and Move bullets
				that.bullets.forEach(function(b,bIndex){
					if(b.dt > that.bulletLife){
						that.bullets.splice(bIndex,1);
						return;
					}
					b.dt += dt;
					b.x += dt*b.v*Math.cos(b.rot);
					b.y += dt*b.v*Math.sin(b.rot);
					b.x %= 1000;
					b.y %= 1000;
					if(b.x < 0) b.x += 1000;
					if(b.y < 0) b.y += 1000;
				});
				
				//Move asteroids
				that.asteroids.forEach(function(a){
					a.x += dt*that.asteroidSpeed*Math.cos(a.rot);
					a.y += dt*that.asteroidSpeed*Math.sin(a.rot);
					a.x %= 1000;
					a.y %= 1000;
					if(a.x < 0) a.x += 1000;
					if(a.y < 0) a.y += 1000;
				});
				
				//Move ship
				
				//Calc previous step stuff
				var cos = Math.cos(that.ship.rot);
				var sin = Math.sin(that.ship.rot);
				var v2 = Math.pow(that.ship.vx,2) + Math.pow(that.ship.vy,2);
				var vx = that.ship.vx;
				var vy = that.ship.vy;
				
				if(that.keysPressed["KeyW"]){
					//accelerate
					that.ship.vx += dt*that.ship.accelRate*cos;
					that.ship.vy += dt*that.ship.accelRate*sin;
				}
				
				//Calc total speed
				
				//Cap speed
				if(v2 > Math.pow(that.ship.maxSpeed,2)){
					that.ship.vx = Math.sign(vx)*Math.pow(vx,2)/v2*that.ship.maxSpeed;
					that.ship.vy = Math.sign(vy)*Math.pow(vy,2)/v2*that.ship.maxSpeed;
				}
				//deccelerate
				if(v2 != 0){
					that.ship.vx -= Math.sign(vx)*dt*that.ship.accelRate*that.damping*(Math.pow(vx,2))/v2;
					that.ship.vy -= Math.sign(vy)*dt*that.ship.accelRate*that.damping*(Math.pow(vy,2))/v2;
				}
				if(that.keysPressed["KeyA"]){
					//Rotate counterclockwise
					that.ship.rot -= dt*that.ship.spinRate;
					that.ship.rot %= 2*Math.PI;
				} else if(that.keysPressed["KeyD"]){
					//Rotate clockwise
					that.ship.rot += dt*that.ship.spinRate;
					that.ship.rot %= 2*Math.PI;
				}
				
				
				//Move the ship
				that.ship.x += dt*that.ship.vx;
				that.ship.y += dt*that.ship.vy;
				that.ship.x %= 1000;
				that.ship.y %= 1000;
				if(that.ship.x < 0) that.ship.x += 1000;
				if(that.ship.y < 0) that.ship.y += 1000;
				//check if any asteroids collide with a bullet
				that.asteroids.forEach(function(a, aIndex){
					that.bullets.forEach(function(b, bIndex){
						//bullet is in circle
						if((Math.pow(b.x-a.x,2)+Math.pow(b.y-a.y,2)) <= Math.pow(a.r+b.r,2)){
							console.log(a);
							console.log(b);
							console.log("d^2: "+(Math.pow(b.x-a.x,2)+Math.pow(b.y-a.y,2)) + ", r^2: " + Math.pow(a.r+b.r,2));
							that.bullets.splice(bIndex,1);
							that.asteroids.splice(aIndex,1);
							if(a.size === 1){
								that.score += 1;
							} else {
								that.asteroids[that.asteroids.length] = new Asteroid(a.size-1,a.x,a.y);
								that.asteroids[that.asteroids.length] = new Asteroid(a.size-1,a.x,a.y);
							}
						}
					});
				});
				//check if any asteroids collide with that ship
				var left = that.ship.x-that.ship.w/2;
				var right = that.ship.x+that.ship.w/2;
				var top = that.ship.y+that.ship.h/2;
				var bottom = that.ship.y+that.ship.h/2;
				that.asteroids.forEach(function(a){
					var revolvedCircle = rotateRef(a.x, a.y, that.ship.rot, that.ship.x, that.ship.y);
					var closestX = a.x, closestY = a.y;
					if(a.x < left) closestX = left;
					else if(a.x > right) closestX = right;
					if(a.y < top) closestY = top;
					else if(a.y > bottom) closestY = bottom;
					//circle intersects
					if(Math.pow(a.x-closestX,2)+Math.pow(a.y-closestY,2) <= Math.pow(a.r,2)){
						//You dead
						gameOver();
					}
				});
				break;
			case 2: //gameover
				if(that.keysPressed["Enter"]){
					console.log("restart");
					play();
				}
				break;
			case -1:
				if(that.keysPressed["Escape"] && that.keyup){
					that.keyup = false;
					play();
				} else if(!that.keysPressed["Escape"]) {
					that.keyup = true;
				}
				break;
		}
		
	}
	
	var rotateRef = function(x, y, rot, aboutX, aboutY) {
		return {
			x: Math.cos(rot)*(x-aboutX)-Math.sin(rot)*(y-aboutY)+aboutX,
			y: Math.sin(rot)*(x-aboutX)+Math.cos(rot)*(y-aboutY)+aboutY,
		}
	}
	var render = function () {
		//clear and draw border
		that.ctx.clearRect(0,0,canvas.width, canvas.height);
		that.ctx.beginPath();
		that.ctx.rect(0,0,canvas.width, canvas.height);
		that.ctx.stroke();
		
		switch (that.state) {
			case 0: //paused
				that.ctx.font = "30px Arial";
				that.ctx.fillText("Paused", 0, 30);
				that.ctx.fillText("Score: "+that.score, 0, 65);
				break;
			case 1: //playing
				//draw asteroids
				that.asteroids.forEach(function(a){
					that.ctx.beginPath();
					that.ctx.arc((a.x)*scale,(a.y)*scale,(a.r)*scale,0,2*Math.PI);
					that.ctx.stroke();
				});
				
				//draw bullets
				that.bullets.forEach(function(b) {
					//make the bullets bigger and easier to see
					that.ctx.beginPath();
					that.ctx.arc((b.x)*scale, (b.y)*scale, (b.r)*scale,0, 2*Math.PI);
					that.ctx.stroke();
				});
				
				//draw player triangle
				that.ctx.save();
				that.ctx.translate(that.ship.x*scale, that.ship.y*scale);
				that.ctx.rotate(that.ship.rot+Math.PI/2);
				that.ctx.beginPath();
				that.ctx.moveTo((-that.ship.w/2)*scale,(that.ship.h/2)*scale);
				that.ctx.lineTo((0)*scale,(-that.ship.h/2)*scale);
				that.ctx.lineTo((that.ship.w/2)*scale,(that.ship.h/2)*scale);
				that.ctx.lineTo((-that.ship.w/2)*scale,(that.ship.h/2)*scale);
				that.ctx.stroke();
				that.ctx.restore();
				
				//draw level and score
				that.ctx.font = "15px Arial";
				that.ctx.fillText("Score: " + that.score, 0, 15);
				that.ctx.fillText("Time: " + Math.floor(that.time), 0, 30);
				break;
			case 2: //game over
				that.ctx.font = "30px Arial";
				that.ctx.fillText("Game Over", 0, 30);
				that.ctx.fillText("Score: "+that.score, 0, 65);
				that.ctx.fillText("Total Time: " + that.time, 0, 100)
				break;
			case -1: //debug
				that.ctx.font = "10px Arial";
				that.ctx.fillText("W: " + canvas.width, 0, 15);
				that.ctx.fillText("H: "+ canvas.height, 0, 30);
				that.ctx.fillText("Scale: " + scale, 0, 45);
				that.ctx.fillText("Total Time: " + that.time, 0, 100)
				break;
		}
		
	}
}

var Asteroid = function (size, x, y) {
	if(size > 3) size = 3;
	if(size < 1) size = 1;
	this.size = size;
	this.x = x;
	this.y = y;
	this.r = 20;
	this.rot = Math.floor(Math.random() * 360);
	switch(size){
		case 3:
			this.r = 60;
			break;
		case 2:
			this.r = 30;
			break;
		case 1:
			this.r = 15;
			break;
	}
}

var Bullet = function (r, x, y, v, rot) {
	this.dt = 0;
	this.x = x;
	this.y = y;
	this.r = r;
	this.rot = rot;
	this.v = v;
}

var Ship = function (x, y, maxSpeed, accelRate, spinRate) {
	this.spinRate = spinRate; //radians/second
	this.accelRate = accelRate; //units/second^2
	this.maxSpeed = maxSpeed; //units/second
	this.x = x;
	this.y = y;
	this.w = 30;
	this.h = 50;
	this.rot = 3/2*Math.PI;
	this.vx = 0;
	this.vy = 0;
}