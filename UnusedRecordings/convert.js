//jshint esversion:8
//jshint node:true
const fs = require("fs");
// const path = require("path");

const usedNames = [
  "Model",
  "Armature",
  "b_l_wrist",
  "b_l_forearm_stub",
  "b_l_humerus",
  "b_l_shoulder",
  "b_l_shoulder_end",
  "b_l_index1",
  "b_l_index2",
  "b_l_index3",
  "b_l_index_null",
  "b_l_index_null_end",
  "b_l_middle1",
  "b_l_middle2",
  "b_l_middle3",
  "b_l_middle_null",
  "b_l_middle_null_end",
  "b_l_pinky0",
  "b_l_pinky1",
  "b_l_pinky2",
  "b_l_pinky3",
  "b_l_pinky_null",
  "b_l_pinky_null_end",
  "b_l_ring1",
  "b_l_ring2",
  "b_l_ring3",
  "b_l_ring_null",
  "b_l_ring_null_end",
  "b_l_thumb0",
  "b_l_thumb1",
  "b_l_thumb2",
  "b_l_thumb3",
  "b_l_thumb_null",
  "b_l_thumb_null_end",
  "l_palm_center_marker",
  "b_r_wrist",
  "b_r_forearm_stub",
  "b_r_humerus",
  "b_r_shoulder",
  "b_r_shoulder_end",
  "b_r_index1",
  "b_r_index2",
  "b_r_index3",
  "b_r_index_null",
  "b_r_index_null_end",
  "b_r_middle1",
  "b_r_middle2",
  "b_r_middle3",
  "b_r_middle_null",
  "b_r_middle_null_end",
  "b_r_pinky0",
  "b_r_pinky1",
  "b_r_pinky2",
  "b_r_pinky3",
  "b_r_pinky_null",
  "b_r_pinky_null_end",
  "b_r_ring1",
  "b_r_ring2",
  "b_r_ring3",
  "b_r_ring_null",
  "b_r_ring_null_end",
  "b_r_thumb0",
  "b_r_thumb1",
  "b_r_thumb2",
  "b_r_thumb3",
  "b_r_thumb_null",
  "b_r_thumb_null_end",
  "r_palm_center_marker",
];

// let rawdata = fs.readFileSync("./wavingLeft3.json");
// let recording = JSON.parse(rawdata);

// recording.serializedFrameTransforms.forEach((serializedFrameTransform) => {
//   serializedFrameTransform.transforms.forEach((transform) => {
//     const data = transform.data;

//     const [a, b, c, d, e, f, g] = data;

//     transform.data = [a, b, c, d, e, f, g];
//   });
// });

// fs.writeFileSync("./wavingLeft3Compressed.json", JSON.stringify(recording));

// Make an async function that gets executed immediately

// Our starting point
// Get the files as an array
const files = fs.readdirSync("./");

console.log(files);

// Loop them all with the new for...of
for (const file of files) {
  if (!file.includes(".json")) continue;

  try {
    let rawdata = fs.readFileSync(file);
    let recording = JSON.parse(rawdata);

    recording.serializedFrameTransforms.forEach(
      (serializedFrameTransform, index) => {
        serializedFrameTransform.transforms.forEach((transform) => {
          const data = transform.data;

          const [a, b, c, d, e, f, g] = data;

          transform.data = [a, b, c, d, e, f, g];
        });
      }
    );

    recording.serializedFrameTransforms =
      recording.serializedFrameTransforms.map((serializedFrameTransform) => ({
        transforms: serializedFrameTransform.transforms.filter((transform) =>
          usedNames.includes(transform.n)
        ),
      }));

    fs.writeFileSync(`../Compressed/${file}`, JSON.stringify(recording));
  } catch (error) {
    console.log(error);
    console.log(file);
  }
} // End for...of
