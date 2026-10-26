import { describe, expect, it } from "vitest";
import type { Camera } from "../types/camera";
import { groupCameras } from "./cameraColumns";

const camera = (number: number): Camera => ({
  number,
  name: `UTR-CM-${number} Test`,
  latitude: 52.09,
  longitude: 5.11,
});

const numbersPerColumn = (numbers: number[]) =>
  groupCameras(numbers.map(camera)).map((column) =>
    column.cameras.map((c) => c.number),
  );

describe("groupCameras", () => {
  it("puts numbers on the boundaries in the right column", () => {
    const result = numbersPerColumn([599, 600, 699, 700, 799, 800]);

    expect(result).toEqual([[599], [600, 699], [700, 799], [800]]);
  });

  it("puts all numbers above 799 in the last column", () => {
    expect(numbersPerColumn([1001, 9999])[3]).toEqual([1001, 9999]);
  });

  it("always returns every column, even when empty", () => {
    expect(groupCameras([])).toHaveLength(4);
  });

  it("keeps the order of the input within a column", () => {
    expect(numbersPerColumn([510, 501, 505])[0]).toEqual([510, 501, 505]);
  });
});
