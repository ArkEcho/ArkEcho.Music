
var sound;

function Init(Text) {
    sound = new Howl({
        src: ['https://localhost:5001/api/Music/MusicFile/' + Text],
        html5: true
    });
}

function PlayAudio() {
    sound.play();
}

function PauseAudio() {
    sound.pause();
}