import { WebsocketService }from './websocketservice';

let wsservice = new WebsocketService();

import http = require('http');
http.createServer((request, response) => {
    response.write('Hello World from Node.js!');
    response.end();
}).listen(3000);
