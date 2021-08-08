/*
 TODO
 */

var sound;
var id;

function InitAudio(Source) {
    sound = new Howl({
        src:[Source],
        html5: true,
        onend: function () {
            console.log('´Sound Finished!'); // Mehr logging -> direkt bei Funktionen auch noch?
            AudioEnd();
        },
        onplay: function () {
            console.log('Sound Play!');
        },
        onpause: function () {
            console.log('Sound Pause!');
        },
        onseek: function () {
            console.log('Sound Seeked!');
        },
        onmute: function () {
            console.log('Sound (Un)Mute!');
        }
    });
}

function DisposeAudio() {
    if (sound != undefined) {
        sound.stop(id);
        sound.unload(id);
        id = 0;
    }
}

function PlayAudio() {
    if (sound != undefined) {
        id = sound.play(id);
    }
}

function PauseAudio() {
    if (sound != undefined) {
        sound.pause(id);
    }
}

function PlayPauseAudio() {
    if (sound != undefined) {
        if (sound.playing(id))
            PauseAudio();
        else
            PlayAudio();
    }
}

function StopAudio() {
    if (sound != undefined) {
        sound.stop(id);
    }
}

function SetAudioMute(Mute) {
    if (sound != undefined) {
        sound.mute(Mute, id);
    }
}

function SetAudioVolume(NewVolume) {
    if (sound != undefined) {
        sound.volume(NewVolume / 100, id);
    }
}

function AudioEnd() {
    console.log('Audio Ended!');
    document.getElementById("AudioEnd").click();
}