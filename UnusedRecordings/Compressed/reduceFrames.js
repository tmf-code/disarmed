const fs = require("fs");
// const files = fs.readdirSync("./");

const act2Endfiles = [
  "posing3Left.json",
  "purchasesLeft.json",
  "smoking2Left.json",
  "directions3Right.json",
  "discussing5Right.json",
  "eatingRight.json",
  "greeting2Right.json",
  "photo2Right.json",
];

const files = [
  "sub1Right.json",
  "sub2Right.json",
  "sub3Right.json",
  "sub4Right.json",
  "sub5Right.json",
  "sub6Right.json",
  "sub7Right.json",
  "sub8Right.json",
  "sub9Right.json",
  "sub10Right.json",
  "sub11Left.json",
  "sub12Left.json",
  "sub13Left.json",
  "sub14Left.json",
  "sub15Left.json",
  "sub16Left.json",
  "sub17Left.json",
  "sub18Left.json",
  "sub19Left.json",
];

console.log(files);

for (const file of files) {
  if (!file.includes(".json")) continue;
  if (file.includes(".meta")) continue;
  // if (!file.includes("waving")) continue;

  try {
    let rawdata = fs.readFileSync(file);
    clipFrames(rawdata, file, 0, 100);
  } catch (error) {
    console.log(error);
    console.log(file);
  }
} // End for...of

function clipFrames(rawdata, file, start, end) {
  let recording = JSON.parse(rawdata);
  recording.serializedFrameTransforms =
    recording.serializedFrameTransforms.slice(start, end);
  fs.writeFileSync(`../FrameClipped/${file}`, JSON.stringify(recording));
}
