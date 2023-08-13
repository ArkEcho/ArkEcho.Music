

//let audio = document.createElement('audio');

class AudioPlayer {
    constructor() {
        this.stepCount = 0;
        this.netObject = null;
        this.stopProgress = false;
        this.audio = document.createElement('audio');
    }

    /* Called by .NET */
    Init(NETObject) {
        this.netObject = NETObject;

        if ("mediaSession" in navigator) {
            navigator.mediaSession.setActionHandler("play", () => {
                //this.log("Browser Play");
                this.netObject.invokeMethodAsync('BrowserPlayPause');
            });
            navigator.mediaSession.setActionHandler("pause", () => {
                //this.log("Browser Pause");
                this.netObject.invokeMethodAsync('BrowserPlayPause');
            });
            // Stop is kinda buggy? After pressing Stop you can't control the audio anymore via the MediaSession
            //navigator.mediaSession.setActionHandler("stop", () => {
            //    //this.log("Browser Stop");
            //    this.netObject.invokeMethodAsync('BrowserStop');
            //});
            navigator.mediaSession.setActionHandler("previoustrack", () => {
                //this.log("Browser Previous");
                this.netObject.invokeMethodAsync('BrowserPreviousTrack');
            });
            navigator.mediaSession.setActionHandler("nexttrack", () => {
                //this.log("Browser Next");
                this.netObject.invokeMethodAsync('BrowserNextTrack');
            });
        }
    }

    /* Called by .NET */
    InitAudio(source, directPlay, volume, mute) {

        this.audio.src = source;
        this.audio.muted = mute;
        this.audio.volume = volume / 100;

        this.audio.onplaying = function () {
            Player.netObject.invokeMethodAsync('AudioPlayingJS', true);
        };
        this.audio.onpause = function () {
            Player.netObject.invokeMethodAsync('AudioPlayingJS', false);
        };
        this.audio.onended = function () {
            Player.netObject.invokeMethodAsync('AudioPlayingJS', false);
            Player.netObject.invokeMethodAsync('AudioEndedJS');
        };

        if (directPlay) {
            this.PlayAudio();
        }
    }


    /* Called by .NET */
    SetDocumentTitle(pageTitle) {
        // Change Title in Browser Tab
        document.title = pageTitle;
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

    /* Called by .NET */
    DisposeAudio() {
        this.stopProgress = true;
        this.StopAudio();
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
    StopAudio() {
        this.PauseAudio();
        this.audio.currentTime = 0;
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