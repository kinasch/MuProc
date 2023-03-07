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
                let currName = notes[i].name;
                
                if(currName.includes("4")){
                    if(typeof (amountOfNotes[currName]) === "undefined"){
                        amountOfNotes[currName] = 0;
                    }
                    amountOfNotes[currName] += 1;
                }

                if(i+1 >= notes.length) break;

                currName = currName.slice(0,currName.length-1).replace("-","");

                if(typeof (notesAndNeighbours[currName]) === "undefined"){
                    notesAndNeighbours[currName] = new Array();
                }
                var nextNoteName = notes[i+1].name;
                // Note down the neighbour of the current note
                notesAndNeighbours[currName].push(nextNoteName.slice(0,nextNoteName.length-1).replace("-",""));
            }
        });
        fileNumber++;
    } catch (error) {
        console.log(fileNumber);
        console.warn(error);
    }
});

let chanceArray = {};
try {
    for (const [key, value] of Object.entries(notesAndNeighbours)) {
        chanceArray[key] = returnChanceArray(key);
        console.log(key+": ");
        console.log(chanceArray[key]);
        console.log("#############################");
    }
} catch (error) {
    console.warn(error);
}

function returnChanceArray(name){
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
const data = JSON.stringify(chanceArray);

// write JSON string to a file
fs.writeFileSync('result.json', data, err => {
  if (err) {
    console.error(err);
  }
  console.log('JSON data is saved.');
});

currTime = (Date.now()-currTime) / 1000;
console.log(`Done with ${fileNumber} after ${currTime}s`);