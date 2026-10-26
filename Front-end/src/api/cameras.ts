import type { Camera } from "../types/camera";

const baseUrl: string = import.meta.env.VITE_API_URL ?? "http://localhost:5080";

export async function fetchCameras(signal?: AbortSignal): Promise<Camera[]> {
  const response = await fetch(`${baseUrl}/api/cameras`, { signal });

  if (!response.ok) {
    throw new Error(
      `The API responded with ${response.status} ${response.statusText}.`,
    );
  }

  return (await response.json()) as Camera[];
}
