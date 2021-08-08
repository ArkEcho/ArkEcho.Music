
var sound;
var id;

function Init(Text) {
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
        }
    });
}

function PlayAudio() {
    if (sound != undefined) {
        id = sound.play();
        //sound.seek(12000, id2);
    }
}

function PauseAudio() {
    if (sound != undefined)
        sound.pause();
}

function onVolumePlayerChanged() {
    if(sound != undefined)
        sound.volume(getVolumeForHowler());
}

function getVolumeForHowler() {
    return document.getElementById("volumePlayer").value / 100;
}

function audioEnd() {
    console.log('AudioFinished!');
    document.getElementById("AudioFinished").click();
}