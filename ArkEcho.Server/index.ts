console.log('Hello world');

import http = require('http');

http.createServer((request, response) => {
    response.write('Hello from Node.js!');
    response.end();
}).listen(3000);
