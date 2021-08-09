/*
 TODO
 */

var sounds = new Array(0);
function firstSound() { return sounds[0]; }
function lastSound() { return sounds[sounds.length - 1]; }

function InitAudio(Source, FileFormat) {
    sounds.push(new Howl({
        src: [Source],
        //preload: true,
        //autoplay: true,
        html5: true,
        format: [FileFormat],
        onend: function () {
            console.log('Finished!'); // Mehr logging -> direkt bei Funktionen auch noch?
            AudioEnd();
        },
        onplay: function () {
            console.log('Play!');
        },
        onpause: function () {
            console.log('Pause!');
        },
        onseek: function () {
            console.log('Seek!');
        },
        onmute: function () {
            console.log('(Un)Mute!');
        },
        onstop: function () {
            console.log('Stop!');
        },
        onload: function () {
            console.log('Load!');
        }
    }));
    console.log('Number of Sounds: ' + sounds.length);
}

function DisposeAudio() {
    if (sounds.length >= 1) {
        var sound = firstSound();

        sound.stop();
        sound.unload();
        sounds.shift();
    }
    console.log('Number of Sounds: ' + sounds.length);
}

function PlayAudio() {
    if (sounds.length >= 1) {
        var sound = lastSound();
        var id = 0;

        if(!sound.playing())
            id = sound.play();

        console.log('Sound Play '+ id);
    }
}

function PauseAudio() {
    if (sounds.length >= 1) {
        var sound = lastSound();
        sound.pause();
    }
}

function PlayPauseAudio() {
    if (sounds.length >= 1) {
        var sound = lastSound();

        if (sound.playing())
            PauseAudio();
        else
            PlayAudio();
    }
}

function StopAudio() {
    if (sounds.length >= 1) {
        var sound = lastSound();
        sound.stop();
    }
}

function SetAudioMute(Mute) {
    if (sounds.length >= 1) {
        var sound = lastSound();
        sound.mute(Mute);
    }
}

function SetAudioVolume(NewVolume) {
    if (sounds.length >= 1) {
        var sound = lastSound();
        sound.volume(NewVolume / 100);
    }
}

function AudioEnd() {
    console.log('Audio Ended!');
    document.getElementById("AudioEnd").click();
}