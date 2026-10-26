import type { Camera } from "../types/camera";

export interface ColumnDefinition {
  id: string;
  title: string;
  /** Decides whether a camera with this number belongs in the column. First matching column wins. */
  matches: (cameraNumber: number) => boolean;
}

export interface CameraColumn {
  id: string;
  title: string;
  cameras: Camera[];
}

export const cameraColumns: ColumnDefinition[] = [
  { id: "below-600", title: "Numbers below 600", matches: (n) => n < 600 },
  { id: "600-699", title: "Numbers 600 to 699", matches: (n) => n < 700 },
  { id: "700-799", title: "Numbers 700 to 799", matches: (n) => n < 800 },
  { id: "other", title: "All other cameras", matches: () => true },
];

/** Spreads cameras over the columns. Each camera lands in the first column that matches its number. */
export function groupCameras(
  cameras: Camera[],
  columns: ColumnDefinition[] = cameraColumns,
): CameraColumn[] {
  const result: CameraColumn[] = columns.map(({ id, title }) => ({
    id,
    title,
    cameras: [],
  }));

  for (const camera of cameras) {
    const index = columns.findIndex((column) => column.matches(camera.number));
    if (index >= 0) {
      result[index].cameras.push(camera);
    }
  }

  return result;
}
