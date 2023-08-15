

let audio = document.createElement('audio');

class AudioPlayer {
    constructor() {
        this.stepCount = 0;
        this.netObject = null;
        this.stopProgress = false;
    }

    /* Called by .NET */
    Init(NETObject) {
        this.netObject = NETObject;
    }

    /* Called by .NET */
    InitAudio(source, directPlay, volume, mute) {

        this.audio = document.createElement('audio');

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
            navigator.mediaSession.setActionHandler("stop", () => {
                // Stop is kinda buggy? After pressing Stop you can't control the audio anymore via the MediaSession UNTIL the user makes an Input
                // It works perfectly fine if Stop is pressed via the HTML Button....
                //this.log("Browser Stop");
                this.netObject.invokeMethodAsync('BrowserStop');
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

        this.audioSrc = document.createElement('source');

        this.audioSrc.setAttribute('src', source);
        this.audioSrc.setAttribute('type', 'audio/mpeg');

        //this.audio.src = source;

        this.audio.append(this.audioSrc);

        this.audio.muted = mute;
        this.audio.volume = volume / 100;

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

        if (directPlay) {
            this.PlayAudio();
        }
        //this.log("Initialized");
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
        this.PauseAudio();

        this.audioSrc.setAttribute('src', '');
        this.audio.remove(this.audioSrc);
        this.audio.src = '';

        const parentAudio = this.audio.parentNode;
        if (parentAudio) {
            parentAudio.removeChild(this.audio);
        }

        const parentSource = this.audioSrc.parentNode;
        if (parentSource) {
            parentSource.removeChild(this.audioSrc);
        }

        this.audio = null;
        this.audioSrc = null;

        //this.log("Audio is " + this.audio);

        if ("mediaSession" in navigator) {
            navigator.mediaSession.setActionHandler("play", null);
            navigator.mediaSession.setActionHandler("pause", null);
            navigator.mediaSession.setActionHandler("stop",null);
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