
var sound;
var id;

function Init(Text) {
    if (sound != undefined) {
        sound.stop();
        sound.unload();
        id = 0;
    }

    sound = new Howl({
        src:['https://localhost:5001/api/Music/MusicFile/' + Text],
        html5: true,
        volume: getVolumeForHowler(),
        onend: function () {
            console.log('Finished!');
            audioEnd();
        },
        onplay: function () {
            console.log('Play!');
        },
        onpause: function () {
            console.log('Pause!');
        },
        onseek: function () {
            console.log('Seeked!');
        },
        onmute: function () {
            console.log('(Un)Mute!');
        }
    });
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

function audioEnd() {
    console.log('Audio Finished!');
    document.getElementById("AudioFinished").click();
}

function onVolumePlayerChanged() {
    if (sound != undefined) {
        sound.volume(getVolumeForHowler(), id);
    }
}

function getVolumeForHowler() {
    return document.getElementById("volumePlayer").value / 100;
}
