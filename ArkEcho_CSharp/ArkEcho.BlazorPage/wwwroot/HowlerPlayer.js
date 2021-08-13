/*
 TODO: Beschreibung mit Howler.js, der Datei hier, dem Knopf zur Event kommunikation etc.
 */

class HowlerPlayer {
    constructor() {
        this.sounds = new Array(0);
        this.stepCount = 0;
        this.LogPlayer('Created new HowlerPlayer!');
    }

    Init(NETObject) {
        this.NetObject = NETObject;
        this.LogPlayer('HowlerPlayer initialized!');
    }

    InitAudio(Source, FileFormat, DirectPlay, Volume, Mute) {
        var self = this;
        this.sounds.push(
            new Howl({
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
                    self.LogPlayer('Sound Ended!');
                },
                onplay: function () {
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
            })
        );

        this.disposed = false;
        this.LogPlayer('Init - Number of Sounds: ' + this.sounds.length);
    }

    // requestAnimationFrame calls this 60/s, limit by Property to invoke "SetPosition" 3/s
    Step() {
        if (this.disposed)
            return;

        if (this.stepCount >= 20) {
            this.stepCount = 0;
            if (this.sounds.length >= 1) {
                var sound = this.Sound();
                if (sound.playing()) {
                    this.NetObject.invokeMethodAsync('SetPositionJS', this.GetAudioPosition());
                }
            }
        }
        this.stepCount++;
        requestAnimationFrame(this.Step.bind(this));
    }

    DisposeAudio() {
        if (this.sounds.length >= 1) {
            this.disposed = true;
            this.sounds[0].unload();
            this.sounds.shift();
            
            this.LogPlayer('Disposed - Number of Sounds: ' + this.sounds.length);
        }
    }

    PlayAudio() {
        if (this.sounds.length >= 1) {
            var sound = this.Sound();
            var id = 0;

            if (!sound.playing())
                id = sound.play();

            this.LogPlayer('Sound Play ID ' + id);
        }
    }

    PauseAudio() {
        if (this.sounds.length >= 1) {
            this.Sound().pause();
            this.LogPlayer('Sound Paused');
        }
    }

    PlayPauseAudio() {
        if (this.sounds.length >= 1) {
            if (this.Sound().playing())
                this.PauseAudio();
            else
                this.PlayAudio();
        }
    }

    StopAudio() {
        if (this.sounds.length >= 1) {
            var sound = this.Sound();
            sound.stop();
            this.LogPlayer('Sound Stop is ' + (sound.seek() == 0 && !sound.playing()));
        }
    }

    SetAudioMute(Mute) {
        if (this.sounds.length >= 1) {
            this.Sound().mute(Mute);
            this.LogPlayer('Sound Mute is ' + Mute);
        }
    }
    SetAudioVolume(NewVolume) {
        if (this.sounds.length >= 1) {
            var sound = this.Sound();
            sound.volume(NewVolume / 100);
            this.LogPlayer('Sound Volume is ' + sound.volume());
        }
    }

    SetAudioPosition(NewTime) {
        if (this.sounds.length >= 1) {
            var sound = this.Sound();
            sound.seek(NewTime);
            this.LogPlayer('Sound Position is ' + sound.seek());
        }
    }

    GetAudioPosition() {
        if (this.sounds.length >= 1) {
            var pos = Math.round(this.Sound().seek());
            this.LogPlayer('Sound Position is ' + pos);
            return pos;
        }
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

    Sound() { return this.sounds[this.sounds.length - 1]; }
}

var Player = new HowlerPlayer();
