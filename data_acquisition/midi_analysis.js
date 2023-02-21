const { Midi } = require('@tonejs/midi');
const fs = require('fs');

let notesAndNeighbours = {};
let amountOfNotes = {};

let files = fs.readdirSync("./dls/");
console.log(files);

files.forEach(file => {
    const midiData = fs.readFileSync("./dls/"+file);
    const midi = new Midi(midiData);
    //the file name decoded from the first track
    console.log(midi.name);
    //get the tracks
    midi.tracks.forEach(track => {
        const notes = track.notes;

        for (let i = 0; i < notes.length; i++) {
            if(i+1 >= notes.length){
                break;
            }
            let currName = notes[i].name;
            // Only the major (non-musical meaning) notes
            currName = currName.slice(0,currName.length-1);
            if(typeof (notesAndNeighbours[currName]) === "undefined"){
                notesAndNeighbours[currName] = new Array();
            }
            // Note down the neighbour of the current note
            notesAndNeighbours[currName].push(notes[i+1].name.slice(0,notes[i+1].name.length-1));

            if(typeof (amountOfNotes[currName]) === "undefined"){
                amountOfNotes[currName] = 0;
            }
            amountOfNotes[currName] += 1;
        }
    });
});

for (const [key, value] of Object.entries(notesAndNeighbours)) {
    console.log(key+": ");
    console.log(prevelanceArray(key));
    console.log("#############################");
}

function prevelanceArray(name){
    let currNoteArr = notesAndNeighbours[name];
    // Notes and their corresponding place in the prevArr
    let correspondingNote = ["A", "A#","B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"];
    // Not using new Array(12) to easily use addition in the following loop.
    let prevArr = [0,0,0,0,0,0,0,0,0,0,0,0];
    for (let i = 0; i < currNoteArr.length; i++) {
        let note = currNoteArr[i];
        prevArr[correspondingNote.indexOf(note)] += 1;
    }

    for (let j = 0; j < prevArr.length; j++) {
        prevArr[j] = (prevArr[j] / currNoteArr.length) * 100;
        prevArr[j] = prevArr[j].toFixed(1)+"%";
    }

    return prevArr;
}

console.log("Done!");