import http from "k6/http";
import { check, sleep } from "k6";

const BASE_URL = "http://localhost:5122";

export const options = {
  stages: [
    { duration: "5s", target: 2000 }, // ramp up to 2000 VUs
    { duration: "10s",  target: 2000 }, // hold at 2000 VUs
    { duration: "5s",  target: 0 }, // ramp down to 0 VUs
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
  sleep(1);
}

