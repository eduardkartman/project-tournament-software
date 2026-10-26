import { useEffect, useState } from "react";
import { fetchCameras } from "../api/cameras";
import type { Camera } from "../types/camera";

export type CamerasState =
  | { status: "loading" }
  | { status: "error"; message: string }
  | { status: "success"; cameras: Camera[] };

export function useCameras(): CamerasState {
  const [state, setState] = useState<CamerasState>({ status: "loading" });

  useEffect(() => {
    const controller = new AbortController();

    fetchCameras(controller.signal)
      .then((cameras) => setState({ status: "success", cameras }))
      .catch((error: unknown) => {
        if (error instanceof DOMException && error.name === "AbortError")
          return;
        const message =
          error instanceof Error ? error.message : "Unknown error.";
        setState({ status: "error", message });
      });

    // Cancels the request if the component unmounts (React StrictMode does this once in development).
    return () => controller.abort();
  }, []);

  return state;
}
