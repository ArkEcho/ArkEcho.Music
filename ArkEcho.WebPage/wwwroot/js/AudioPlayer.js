

//let audio = document.createElement('audio');

class AudioPlayer {
    constructor() {
        this.stepCount = 0;
        this.netObject = null;
        this.stopProgress = false;
    }

    /* Called by .NET */
    Init(NETObject) {
        this.netObject = NETObject;
        this.audio = document.createElement('audio');
    }

    /* Called by .NET */
    InitAudio(chunkSources, completeSource, mimetype, directPlay, volume, mute) {

        this.directPlay = directPlay;        
        this.audio.muted = mute;
        this.audio.volume = volume / 100;

        navigator.mediaSession.playbackState = "paused";
        if ("mediaSession" in navigator) {

            //this.log("Setting Mediasession Handler");
            navigator.mediaSession.setActionHandler("play", () => {
                //this.log("Browser Play");
                this.netObject.invokeMethodAsync('BrowserPlayPause');
            });
            navigator.mediaSession.setActionHandler("pause", () => {
                //this.log("Browser Pause");
                this.netObject.invokeMethodAsync('BrowserPlayPause');
            });
            navigator.mediaSession.setActionHandler("previoustrack", () => {
                //this.log("Browser Previous");
                this.netObject.invokeMethodAsync('BrowserPreviousTrack');
            });
            navigator.mediaSession.setActionHandler("nexttrack", () => {
                //this.log("Browser Next");
                this.netObject.invokeMethodAsync('BrowserNextTrack');
            });
        }
        this.audio.onplaying = function () {
            Player.netObject.invokeMethodAsync('AudioPlayingJS', true);
            navigator.mediaSession.playbackState = "playing";
        };
        this.audio.onpause = function () {
            Player.netObject.invokeMethodAsync('AudioPlayingJS', false);
            navigator.mediaSession.playbackState = "none";
        };
        this.audio.onended = function () {
            Player.netObject.invokeMethodAsync('AudioPlayingJS', false);
            Player.netObject.invokeMethodAsync('AudioEndedJS');
            navigator.mediaSession.playbackState = "none";
        };

        // Function to fetch each audio file and concatenate them
        function fetchAndConcatenateAudio(chunkSources, completeSource, mimetype) {
            var xhr = new XMLHttpRequest();
            var blobs = [];
            var errorcounter = 0;
            var loadComplete = false;

            xhr.onload = function () {
                if (xhr.status === 200) {
                    if (!loadComplete) {
                        blobs.push(xhr.response);
                        if (blobs.length === chunkSources.length) {

                            // Concatenate all blobs
                            var concatenatedBlob = new Blob(blobs, { type: mimetype });
                            blobs = [];
                            // Create object URL and set it as the source for the audio element
                            Player.SetAudioSourceAndPlay(concatenatedBlob);
                        }
                        else {
                            // If there are more URLs, fetch the next one
                            fetchChunkBlob();
                        }
                    }
                    else {
                        Player.SetAudioSourceAndPlay(xhr.response);
                    }
                }
                else if (xhr.status === 403) { // Forbidden, Anti Virus etc.
                    errorcounter++;
                    if (!loadComplete) { // Try loading the whole Audio as one
                        loadComplete = true;
                        fetchComplete();
                    }
                    else { // Chunk and whole Audio failed
                        Player.FatalError("Loading of audio failed!");
                    }
                }
                else { // Error
                    errorcounter++;
                    if (errorcounter < 1) { // Try again
                        fetchChunkBlob(); 
                    }
                    else if (errorcounter < 2) { // If failed twice, try loading the whole Audio as one
                        loadComplete = true;
                        fetchComplete();
                    }
                    else { // Chunk and whole Audio failed
                        Player.FatalError("Loading of audio failed!");
                    }
                }                
            };

            function fetchComplete() {
                xhr.open("GET", completeSource, true);
                xhr.responseType = mimetype;
                xhr.send();
            }

            function fetchChunkBlob() {
                xhr.open("GET", chunkSources[blobs.length], true);
                xhr.responseType = "blob";
                xhr.send();
            }

            // Start fetching the first blob
            fetchChunkBlob();
        }

        // Start fetching and concatenating audio files
        fetchAndConcatenateAudio(chunkSources, completeSource, mimetype);
    }

    FatalError(error) {
        Player.netObject.invokeMethodAsync('AudioFatalErrorJS', error);
    }
    
    SetAudioSourceAndPlay(audioBlob) {
        this.audioBlob = audioBlob;
        this.audioUrl = URL.createObjectURL(this.audioBlob);
        this.audio.src = this.audioUrl;
        if (this.directPlay) {
            this.PlayAudio();
        }
    }
    
    /* Called by .NET */
    SetDocumentTitle(pageTitle) {
        // Change Title in Browser Tab
        document.title = pageTitle;
    }

    /* Called by .NET */
    DisposeAudio() {
        this.audio.pause();
        this.stopProgress = true;
        
        this.audio = document.createElement('audio');
        URL.revokeObjectURL(this.audioUrl);
        this.audioBlob = null;

        if ("mediaSession" in navigator) {
            navigator.mediaSession.setActionHandler("play", null);
            navigator.mediaSession.setActionHandler("pause", null);
            navigator.mediaSession.setActionHandler("previoustrack", null);
            navigator.mediaSession.setActionHandler("nexttrack",null);
        }
        navigator.mediaSession.setPositionState(null);
        navigator.mediaSession.playbackState = "none";
        //this.log("Disposed");
    }

    /* Called by .NET */
    PlayAudio() {
        if (this.audio.paused) {
            this.audio.play();
            this.stopProgress = false;
            requestAnimationFrame(this.Step.bind(this));
        }
    }

    /* Called by .NET */
    PauseAudio() {
        if (!this.audio.paused) {
            this.audio.pause();
            this.stopProgress = true;
        }
    }

    /* Called by .NET */
    SetAudioMute(mute) {
        this.audio.muted = mute;
    }

    /* Called by .NET */
    SetAudioVolume(volume) {
        this.audio.volume = volume / 100;
    }

    /* Called by .NET */
    SetAudioPosition(newTime) {
        this.audio.currentTime = newTime;
    }

    /* Called by .NET */
    GetAudioPosition() {
        let pos = Math.round(this.audio.currentTime);
        return pos;
    }

    // requestAnimationFrame calls this 60/s, limit by Property to invoke "SetPosition" 3/s
    Step() {
        if (this.stopProgress)
            return;

        if (this.stepCount >= 20) {
            this.stepCount = 0;                
            if (!this.audio.paused) {
                this.netObject.invokeMethodAsync('AudioPositionChangedJS', this.GetAudioPosition());
            }
        }
        this.stepCount++;
        requestAnimationFrame(this.Step.bind(this));
    }

    log(text) {
        console.log('[' + this.getCurrentDateTime() + '] ' + text);
    }

    getCurrentDateTime() {
        let currentdate = new Date();
        let datetime = currentdate.getFullYear() + "-"
            + ((currentdate.getMonth() + 1) < 10 ? "0" : "") + (currentdate.getMonth() + 1) + "-"
            + (currentdate.getDate() < 10 ? "0" : "") + currentdate.getDate() + "_"
            + (currentdate.getHours() < 10 ? "0" : "") + currentdate.getHours() + ":"
            + currentdate.getMinutes() + ":"
            + currentdate.getSeconds() + "."
            + currentdate.getMilliseconds();
        return datetime;
    }
}

var Player = new AudioPlayer();