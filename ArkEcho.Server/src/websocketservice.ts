
import * as WebSocket from "ws"

export class WebsocketService {
    
    private wss : WebSocket.Server;
    
    constructor(port: number)
    { 
        this.wss = new WebSocket.Server({port: port});
                
        this.wss.on('connection', function connection(ws, req) 
        {      
            ws.on('close', function close() 
            {
                console.log('Socket Disconnected: ' + ws.url);
            });
            
            ws.on('message', function incoming(message) 
            {
                console.log("Received Message from " + ws.url + ": " + message);
                ws.send('Hello World!');      
            });
        });
    }

    public CloseServer(){
        this.wss.close();
    }
}
