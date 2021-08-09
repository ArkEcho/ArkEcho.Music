/*
 TODO
 */

var sound;
var id;

function InitAudio(Source, FileFormat) {
    sound = new Howl({
        src: [Source],
        //preload: true,
        //autoplay: true,
        html5: true,
        //format: [FileFormat],
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
        },
        onstop: function () {
            console.log('Sound Stopped!');
        },
        onload: function () {
            console.log('Sound Loaded!');
        }
    });
    //console.log('Sound is ' + (sound != undefined));
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
        //console.log('Play for ID: ' + id);
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