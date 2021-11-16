//jshint esversion:8
//jshint node:true
const fs = require("fs");
// const path = require("path");
const files = fs.readdirSync("./");

// Loop them all with the new for...of
for (const file of files) {
  if (!file.includes(".json")) continue;
  if (file.includes(".meta")) continue;
  if (file.endsWith("Left.json") || file.endsWith("Right.json")) {
    continue;
  } else {
    fs.unlinkSync(file);
  }
} // End for...of
// for (const file of files) {
//   if (!file.includes(".json")) continue;
//   if (file.includes(".meta")) continue;

//   try {
//     let rawdata = fs.readFileSync(file);
//     let recording = JSON.parse(rawdata);
//     if (recording.hand === 0) {
//       fs.writeFileSync(
//         `../Compressed/${file.replace(".json", "Left.json")}`,
//         JSON.stringify(recording)
//       );
//     } else if (recording.hand === 1) {
//       fs.writeFileSync(
//         `../Compressed/${file.replace(".json", "Right.json")}`,
//         JSON.stringify(recording)
//       );
//     }
//   } catch (error) {
//     console.log(error);
//     console.log(file);
//   }
// } // End for...of
