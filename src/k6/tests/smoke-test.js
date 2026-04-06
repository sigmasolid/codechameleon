import http from "k6/http";
import { check, sleep } from "k6";

const BASE_URL = "http://localhost:5122";

export const options = {
  duration: "10s",

  // The smoke test must pass every single check — anything less is a red flag.
  thresholds: {
    checks: ["rate==1.00"],
    http_req_failed: ["rate==0"],
  },
};

export default function () {
  // ── GET /products ──────────────────────────────────────────────────────────
  const productsRes = http.get(`${BASE_URL}/products`);

  check(productsRes, {
    "products: status is 200": (r) => r.status === 200,
    "products: returns an array": (r) => Array.isArray(r.json()),
    "products: array is not empty": (r) => r.json().length > 0,
  });

  // ── GET /products/{id} ─────────────────────────────────────────────────────
  const id = 42;
  const productRes = http.get(`${BASE_URL}/products/${id}`);

  check(productRes, {
    "product by id: status is 200": (r) => r.status === 200,
    "product by id: id matches request": (r) => r.json("id") === id,
    "product by id: has a name": (r) =>
      typeof r.json("name") === "string" && r.json("name").length > 0,
    "product by id: has a price": (r) => r.json("price") > 0,
  });

  sleep(1);
}

