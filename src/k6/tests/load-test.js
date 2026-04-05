/**
 * Load Test
 * ---------
 * Purpose : Simulate a realistic, sustained traffic pattern and verify the API
 *           meets its performance targets under normal load.
 *
 * Stages  :
 *   0 → 20 VUs  over 30 s  (ramp-up)
 *   20 VUs      for  1 min (steady load)
 *   20 → 0 VUs  over 20 s  (ramp-down)
 *
 * Thresholds (the test FAILS if any of these are breached):
 *   • 95th-percentile response time < 500 ms
 *   • HTTP error rate < 1 %
 *   • At least 95 % of checks must pass
 *
 * Run:
 *   k6 run tests/load-test.js
 */

import http from "k6/http";
import { check, sleep } from "k6";

const BASE_URL = "http://localhost:5122";

export const options = {
  stages: [
    { duration: "5s", target: 20 }, // ramp up to 20 VUs
    { duration: "10s",  target: 20 }, // hold at 20 VUs
  ],

  thresholds: {
    // 95 % of all requests must complete within 500 ms
    http_req_duration: ["p(95)<500"],
    // Less than 1 % of requests may fail
    http_req_failed: ["rate<0.01"],
    // 95 % of checks must pass
    checks: ["rate>0.95"],
  },
};

export default function () {
  // Each VU cycles through all endpoints in every iteration

  // ── GET /products ──────────────────────────────────────────────────────────
  const productsRes = http.get(`${BASE_URL}/products`);

  check(productsRes, {
    "products: status is 200": (r) => r.status === 200,
    "products: returns an array": (r) => Array.isArray(r.json()),
  });

  // ── GET /products/{id} ─────────────────────────────────────────────────────
  // Use a random id so different VUs naturally request different products
  const id = Math.floor(Math.random() * 1000) + 1;
  const productRes = http.get(`${BASE_URL}/products/${id}`);

  check(productRes, {
    "product by id: status is 200": (r) => r.status === 200,
    "product by id: id matches request": (r) => r.json("id") === id,
  });

  // Pause between iterations to mimic a real user's think time
  sleep(0.1);
}

