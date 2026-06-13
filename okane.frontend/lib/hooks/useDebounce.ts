import { useCallback, useState } from "react";

export default function useDebounce(callback: (query: string) => Promise<void>) {
    const [timeoutRef, setTimeoutRef] = useState<NodeJS.Timeout | null>(null);

    const debounced = useCallback((query: string) => {
        if(timeoutRef != null) {
            clearTimeout(timeoutRef);
        }
        setTimeoutRef(setTimeout(() => callback(query), 200));
    }, [callback, timeoutRef]);

    return {
        debounced
    }
}