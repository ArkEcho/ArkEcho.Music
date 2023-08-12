

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
    }

    /* Called by .NET */
    InitAudio(source, directPlay, volume, mute) {

        this.audio.src = source;
        this.audio.muted = mute;
        this.audio.volume = volume / 100;

        this.audio.onplaying = function () {
            player.netObject.invokeMethodAsync('AudioPlayingJS', true);
        };
        this.audio.onpause = function () {
            player.netObject.invokeMethodAsync('AudioPlayingJS', false);
        };
        this.audio.onended = function () {
            player.netObject.invokeMethodAsync('AudioPlayingJS', false);
            player.netObject.invokeMethodAsync('AudioEndedJS');
        };
        this.audio.onstop = function () {
            player.netObject.invokeMethodAsync('AudioPlayingJS', false);
        };

        if (directPlay) {
            this.audio.play()
                .then(_ => {
                    this.stopProgress = false;
                    this.netObject.invokeMethodAsync('AudioPlayingJS', true);
                    requestAnimationFrame(this.Step.bind(this));
                }
                )
                .catch(error => this.log(error));
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
        if (this.audio.paused)
            this.audio.play();
    }

    /* Called by .NET */
    PauseAudio() {
        if (!this.audio.paused)
            this.audio.pause();
    }

    /* Called by .NET */
    StopAudio() {
        this.stopProgress = true;
        this.audio.pause();
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

var player = new AudioPlayer();