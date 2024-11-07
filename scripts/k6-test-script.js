// winget install k6 --source winget
// k6 run scripts/k6-test-script.js

import http from 'k6/http';
import { sleep } from 'k6';
import { SharedArray } from 'k6/data';

// Set options for virtual users and test duration
export const options = {
  vus: 5,           // Number of virtual users
  duration: '10s',  // Test duration
};

const userNames = new SharedArray('userNames', function () {

    // create an array of 5 unique usernames
    const userNames = [];
    for (let i = 0; i < 5; i++) {
      userNames.push(`user${i}`);
    }

    return userNames;
});

export default function () {
  // Define the URL and JSON payload
  const url = 'https://localhost:7111/maintenance-plan';
  const payload = JSON.stringify({
    productId: '45a340d8-7bd1-4f56-95b3-8ca192ef6094',
    customerId: '4f8b1c36-047b-4c5c-99e4-bf5446856d14',
    description: 'Plan for my Widget',
    userName: userNames[Math.floor(Math.random() * userNames.length)],
    effectiveOn: '2024-11-09T00:00:00.000Z',
  });

  // Set headers for JSON content
  const params = {
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    },
  };

  // Send the POST request
  http.post(url, payload, params);

  // Pause briefly between requests to simulate real user activity
  sleep(1);
}
