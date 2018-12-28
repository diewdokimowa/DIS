const CELL_SIZE = 10;
let currentGeneration = [];
let nextGeneration = [];
let canvas;
let context;
let generationValue;

window.onload = Initialize;

function Initialize() {
    generationValue = document.getElementById('generation');
    canvas = document.getElementById('game');
    canvas.width = 1280;
    canvas.height = 720;
    context = canvas.getContext('2d');
    
    function Game() {
        let gameLoop = -1;
        let generation = 0;
        this.board = {
            width: Math.round(1280 / CELL_SIZE),
            height: Math.round(720 / CELL_SIZE)
        };
        for (let i = 0; i < this.board.width; i++) {
            currentGeneration[i] = [];
            nextGeneration[i] = [];
            for (let j = 0; j < this.board.height; j++) {
                currentGeneration[i][j] = false;
                nextGeneration[i][j] = false;
            }
        }

        this.renderCell = function (x, y) {
            context.fillRect(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE + 1, CELL_SIZE + 1);    
        };
        this.drawGrid = function () {
            context.beginPath();
            context.lineWidth = 1;
            context.strokeStyle = "#dddddd";
            for (let i = 0; i <= this.board.width; i++) {
                context.moveTo(0, i * CELL_SIZE);
                context.lineTo(canvas.width, i * CELL_SIZE);
                context.moveTo(i * CELL_SIZE, 0);
                context.lineTo(i * CELL_SIZE, canvas.height);
            }
            context.stroke();
        };
        this.processGeneration = function () {
            context.clearRect(0, 0, canvas.width, canvas.height);
            this.drawGrid();
            for (let i = 0; i < this.board.width; i++) {
                for (let j = 0; j < this.board.height; j++) {
                    if (currentGeneration[i][j]) {
                        this.renderCell(i, j);
                    }
                }
            }
            generationValue.innerText = generation;

            for (let i = 0; i < this.board.width; i++) {
                for (let j = 0; j < this.board.height; j++) {
                    let result = false;
                    const alive = currentGeneration[i][j];
                    const count = this.getNeighbors(i, j);

                    if (alive && count < 2) {
                        result = false;
                    }
                    else if (alive && (count === 2 || count === 3)) {
                        result = true;
                    }
                    else if (alive && count > 3) {
                        result = false;
                    }
                    else if (!alive && count === 3) {
                        result = true;
                    }
                    
                    nextGeneration[i][j] = result;
                }
            }
            
            for (let i = 0; i < this.board.width; i++) {
                for (let j = 0; j < this.board.height; j++) {
                    currentGeneration[i][j] = nextGeneration[i][j];
                }
            }

            generation++;
        };
        this.randomizeGameBoard = function () {
            context.clearRect(0, 0, canvas.width, canvas.height);
            this.drawGrid();
            generation = 0;
            
            for (let i = 0; i < this.board.width; i++) {
                for (let j = 0; j < this.board.height; j++) {
                    const alive = !Math.round(Math.random());
                    currentGeneration[i][j] = alive;
                    if (alive) {
                        this.renderCell(i, j);
                    }
                }
            }
        };
        this.stopStart = function () {
            if (gameLoop === -1) {
                gameLoop = setInterval(this.processGeneration.bind(this), 100);
            } else {
                clearInterval(gameLoop);
                gameLoop = -1;
            }
        };
        this.getNeighbors = function (x, y) {
            let count = 0;
            const boardWidth = this.board.width;
            const boardHeight = this.board.height;

            if (x !== 0 && y !== 0 && currentGeneration[x - 1][y - 1]) {
                count++;
            }
            if (y !== 0 && currentGeneration[x][y - 1]) {
                count++;
            }
            if (x !== boardWidth - 1 && y !== 0 && currentGeneration[x + 1][y - 1]) {
                count++;
            }
            if (x !== 0 && currentGeneration[x - 1][y]) {
                count++;
            }
            if (x !== boardWidth - 1 && currentGeneration[x + 1][y]) {
                count++;
            }
            if (x !== 0 && y !== boardHeight - 1 && currentGeneration[x - 1][y + 1]) {
                count++;
            }
            if (y !== boardHeight - 1 && currentGeneration[x][y + 1]) {
                count++;
            }
            if (x !== boardWidth - 1 && y !== boardHeight - 1 && currentGeneration[x + 1][y + 1]) {
                count++;
            }
            
            return count;
        };
        this.clear = function () {
            for (let i = 0; i < this.board.width; i++) {
                for (let j = 0; j < this.board.height; j++) {
                    currentGeneration[i][j] = false;
                }
            }
            generation = 0;
            clearInterval(gameLoop);
            gameLoop = -1;
            this.processGeneration();
        };
    }

    const game = new Game();
    game.processGeneration();
    game.drawGrid();

    canvas.addEventListener('click', function(event) {
        const x = Math.round(event.clientX / CELL_SIZE);
        const y = Math.round(event.clientY / CELL_SIZE);
        game.renderCell(x, y);
        currentGeneration[x][y] = true;
    });
    
    const clearButton = document.getElementById('clear');
    clearButton.addEventListener('click', game.clear.bind(game));
    
    const generateButton = document.getElementById('generate');
    generateButton.addEventListener('click', game.randomizeGameBoard.bind(game));
    
    const startStopButton = document.getElementById('start-stop');
    startStopButton.addEventListener('click', game.stopStart.bind(game));
}