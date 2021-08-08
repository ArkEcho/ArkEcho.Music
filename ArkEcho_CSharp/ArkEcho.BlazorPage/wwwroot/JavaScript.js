
var sound;

function Init(Text) {
     sound = new Howl({
         src:['https://localhost:5001/api/Music/MusicFile/' + Text],
         html5: true,
         volume: getVolumeForHowler()
     });
}

function PlayAudio() {
    if (sound != undefined)
        sound.play();
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