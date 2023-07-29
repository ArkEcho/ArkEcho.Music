
class HowlerPlayer {
    constructor() {
        this.stepCount = 0;
        //this.LogPlayer('Created new HowlerPlayer!');
    }

    Init(NETObject) {
        this.NetObject = NETObject;
        //this.LogPlayer('HowlerPlayer initialized!');
        initBrowserInterop();
    }

    initBrowserInterop() {
        if ("mediaSession" in navigator) {
            //navigator.mediaSession.metadata = new MediaMetadata({
            //    title: "Unforgettable",
            //    artist: "Nat King Cole",
            //    album: "The Ultimate Collection (Remastered)",
            //});

            navigator.mediaSession.setActionHandler("play", () => {
                //alert("Play");
                self.NetObject.invokeMethodAsync('BrowserPlayPause');
            });
            navigator.mediaSession.setActionHandler("pause", () => {
                //alert("Pause");
                self.NetObject.invokeMethodAsync('BrowserPlayPause');
            });
            //navigator.mediaSession.setActionHandler("stop", () => {
            //    //alert("Stop");
            //    self.NetObject.invokeMethodAsync('BrowserStop');
            //});

            //navigator.mediaSession.setActionHandler("seekbackward", () => {
            //    alert("seekbackward");
            //    //self.NetObject.invokeMethodAsync('BrowserPreviousTrack');
            //});
            //navigator.mediaSession.setActionHandler("seekforward", () => {
            //    alert("seekforward");
            //    //self.NetObject.invokeMethodAsync('BrowserNextTrack');
            //});
            //navigator.mediaSession.setActionHandler("previoustrack", () => {
            //    alert("PreviousTrack");
            //    //self.NetObject.invokeMethodAsync('BrowserPreviousTrack');
            //});
            //navigator.mediaSession.setActionHandler("nexttrack", () => {
            //    alert("NextTrack");
            //    //self.NetObject.invokeMethodAsync('BrowserNextTrack');
            //});
        }
        else {
            this.LogPlayer('mediasession not found!');
        }
    }

    InitAudio(Source, pageTitle, FileFormat, DirectPlay, Volume, Mute) {
        var self = this;
        this.sound = new Howl({
            //preload: true,
            html5: true,

            src: [Source],
            autoplay: DirectPlay,
            format: [FileFormat],
            volume: Volume / 100,
            mute: Mute,

            onend: function () {
                self.NetObject.invokeMethodAsync('AudioPlayingJS', false);
                self.NetObject.invokeMethodAsync('AudioEndedJS');
                //self.LogPlayer('Sound Ended!');
            },
            onplay: function () {
                self.stop = false;
                self.NetObject.invokeMethodAsync('AudioPlayingJS', true);
                requestAnimationFrame(self.Step.bind(self));
            },
            onpause: function () {
                self.NetObject.invokeMethodAsync('AudioPlayingJS', false);
            },
            onseek: function () {
                //self.LogPlayer('Seek!');
            },
            onmute: function () {
                //self.LogPlayer('(Un)Mute!');
            },
            onstop: function () {
                self.NetObject.invokeMethodAsync('AudioPlayingJS', false);
            },
            onload: function () {
                //self.LogPlayer('Load!');
            },
            onvolume: function () {
                //self.LogPlayer('Volume!');
            }
        });

        // Change Title in Browser Tab
        document.title = pageTitle;

        //this.LogPlayer('Init Audio Succeed');
    }

    // requestAnimationFrame calls this 60/s, limit by Property to invoke "SetPosition" 3/s
    Step() {
        if (this.stop)
            return;

        if (this.stepCount >= 20) {
            this.stepCount = 0;                
            if (this.sound.playing()) {
                this.NetObject.invokeMethodAsync('AudioPositionChangedJS', this.GetAudioPosition());
            }
        }
        this.stepCount++;
        requestAnimationFrame(this.Step.bind(this));
    }

    DisposeAudio() {
        this.stop = true;
        this.sound.unload();
        //this.LogPlayer('Disposed Audio');
    }

    PlayAudio() {
        var id = 0;

        if (!this.sound.playing())
            id = this.sound.play();

        //this.LogPlayer('Sound Play ID ' + id);
    }

    PauseAudio() {
        this.sound.pause();
        //this.LogPlayer('Sound Paused');
    }

    StopAudio() {
        this.stop = true;
        this.sound.stop();
        //this.LogPlayer('Sound Stop is ' + (this.sound.seek() == 0 && !this.sound.playing()));
    }

    SetAudioMute(Mute) {
        this.sound.mute(Mute);
        //this.LogPlayer('Sound Mute is ' + Mute);
    }

    SetAudioVolume(NewVolume) {
        this.sound.volume(NewVolume / 100);
        //this.LogPlayer('Sound Volume is ' + this.sound.volume());
    }

    SetAudioPosition(NewTime) {
        this.sound.seek(NewTime);
        //this.LogPlayer('Sound Position is ' + this.sound.seek());
    }

    GetAudioPosition() {
        var pos = Math.round(this.sound.seek());
        //this.LogPlayer('Sound Position is ' + pos);
        return pos;
    }

    LogPlayer(Text) {
        console.log('[' + this.GetCurrentDateTime() + '] ' + Text);
    }

    GetCurrentDateTime() {
        var currentdate = new Date();
        var datetime = currentdate.getFullYear() + "-"
            + ((currentdate.getMonth() + 1) < 10 ? "0" : "") + (currentdate.getMonth() + 1) + "-"
            + (currentdate.getDate() < 10 ? "0" : "") + currentdate.getDate() + "_"
            + (currentdate.getHours() < 10 ? "0" : "") + currentdate.getHours() + ":"
            + currentdate.getMinutes() + ":"
            + currentdate.getSeconds() + "."
            + currentdate.getMilliseconds();
        return datetime;
    }
}

var Player = new HowlerPlayer();
