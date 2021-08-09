/*
 TODO
 */

var sounds = new Array(0);
function firstSound() { return sounds[0]; }
function lastSound() { return sounds[sounds.length - 1]; }

function InitAudio(Source, FileFormat, DirectPlay, Volume, Mute) {
    sounds.push(
        new Howl({
            //preload: true,
            html5: true,

            src: [Source],
            autoplay: DirectPlay,
            format: [FileFormat],
            volume: Volume / 100,
            mute: Mute,

            onend: function () {
                LogPlayer('Finished!');
                AudioEnd();
            },
            onplay: function () {
                LogPlayer('Play!');
            },
            onpause: function () {
                LogPlayer('Pause!');
            },
            onseek: function () {
                LogPlayer('Seek!');
            },
            onmute: function () {
                LogPlayer('(Un)Mute!');
            },
            onstop: function () {
                LogPlayer('Stop!');
            },
            onload: function () {
                LogPlayer('Load!');
            },
            onvolume: function () {
                LogPlayer('Volume!');
            }
        })
    );
    LogPlayer('Init - Number of Sounds: ' + sounds.length);
}

function DisposeAudio() {
    if (sounds.length >= 1) {
        firstSound().unload();
        sounds.shift();

        LogPlayer('Disposed - Number of Sounds: ' + sounds.length);
    }
}

function PlayAudio() {
    if (sounds.length >= 1) {
        var sound = lastSound();
        var id = 0;

        if(!sound.playing())
            id = sound.play();

        LogPlayer('Sound Play ID '+ id);
    }
}

function PauseAudio() {
    if (sounds.length >= 1) {
        lastSound().pause();
        LogPlayer('Sound Paused');
    }
}

function PlayPauseAudio() {
    if (sounds.length >= 1) {
        if (lastSound().playing())
            PauseAudio();
        else
            PlayAudio();
    }
}

function StopAudio() {
    if (sounds.length >= 1) {
        var sound = lastSound();
        sound.stop();
        LogPlayer('Sound Stop is ' + (sound.seek() == 0 && !sound.playing()));
    }
}

function SetAudioMute(Mute) {
    if (sounds.length >= 1) {
        lastSound().mute(Mute);
        LogPlayer('Sound Mute is ' + Mute);
    }
}

function SetAudioVolume(NewVolume) {
    if (sounds.length >= 1) {
        var sound = lastSound();
        sound.volume(NewVolume / 100);
        LogPlayer('Sound Volume is ' + sound.volume());
    }
}

function SetAudioPosition(NewTime) {
    if (sounds.length >= 1) {
        var sound = lastSound();
        sound.seek(NewTime);
        LogPlayer('Sound Position is ' + sound.seek());
    }
}

function AudioEnd() {
    document.getElementById("AudioEnd").click();
    LogPlayer('Sound Ended!');
}

function LogPlayer(Text) {
    console.log('[' + GetCurrentDateTime() + '] ' + Text);
}

function GetCurrentDateTime() {
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