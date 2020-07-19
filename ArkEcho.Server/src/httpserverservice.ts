import http = require('http');

export class HttpServerService{

    private server: http.Server;

    constructor(port: number){
        this.server = http.createServer((request, response) => {
            response.write('Hello World from Node.js!');
            response.end();
        }).listen(port);
    }

    public CloseServer(){
        this.server.close();
    }
}
