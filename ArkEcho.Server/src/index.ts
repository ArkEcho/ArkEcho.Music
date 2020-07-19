//Console.Log('Hello World');

import http = require('http');

http.createServer((request, response) => {
    response.write('Hello World from Node.js!');
    response.end();
}).listen(3000);
