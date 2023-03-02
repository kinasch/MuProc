const { Midi } = require('@tonejs/midi');
const fs = require('fs');

let notesAndNeighbours = {};
let amountOfNotes = {};

// Technically I should use private.json here as well. Technically.
let files = fs.readdirSync("./dls/");
console.log(files);
let fileNumber = 0;
let currTime = Date.now();


files.forEach(file => {
    try{
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
                if(currName.includes('-')) continue;
                // Only the major (non-musical meaning) notes
                currName = currName.slice(0,currName.length-1);
                if(typeof (notesAndNeighbours[currName]) === "undefined"){
                    notesAndNeighbours[currName] = new Array();
                }
                // Note down the neighbour of the current note
                if(!notes[i+1].name.includes('-')){
                    notesAndNeighbours[currName].push(notes[i+1].name.slice(0,notes[i+1].name.length-1));
                }

                if(typeof (amountOfNotes[currName]) === "undefined"){
                    amountOfNotes[currName] = 0;
                }
                amountOfNotes[currName] += 1;
            }
        });
        fileNumber++;
    } catch (error) {
        console.log(fileNumber);
        console.warn(error);
    }
});

let prevelanceData = {};
try {
    for (const [key, value] of Object.entries(notesAndNeighbours)) {
        prevelanceData[key] = prevelanceArray(key);
        console.log(key+": ");
        console.log(prevelanceData[key]);
        console.log("#############################");
    }
} catch (error) {
    console.warn(error);
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
        prevArr[j] = (prevArr[j] / currNoteArr.length);
        prevArr[j] = parseFloat(prevArr[j].toFixed(4));
    }

    return prevArr;
}

// convert JSON object to a string
const data = JSON.stringify(prevelanceData);

// write JSON string to a file
fs.writeFileSync('result.json', data, err => {
  if (err) {
    console.error(err);
  }
  console.log('JSON data is saved.');
});

currTime = (Date.now()-currTime) / 1000;
console.log(`Done with ${fileNumber} after ${currTime}s`);