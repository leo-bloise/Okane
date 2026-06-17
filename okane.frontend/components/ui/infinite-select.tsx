'use client';

import { ArrowDown } from "lucide-react";
import {
    MouseEventHandler,
    ReactNode,
    UIEventHandler,
    useCallback,
    useEffect,
    useRef,
    useState
} from "react";
import { Spinner } from "./spinner";

type SelectState<T> = {
    items: T[];
    nextToken: string;
};

type Props<T, TValue> = {
    value?: TValue;

    onChange?: (value: TValue) => void;

    getValue(item: T): TValue;

    itemToLiAdapter: (
        data: T,
        onClickListener: MouseEventHandler<HTMLLIElement>,
        index: number,
        selected: boolean
    ) => ReactNode;

    placeholderText?: string;

    source: (
        continuationToken: string
    ) => Promise<{
        items: T[];
        continuationToken: string;
    }>;

    getDisplay(value: T): string;

    merger: (
        oldItems: T[],
        newItems: T[]
    ) => T[];
};

export default function InfiniteSelect<T, TValue>({
    itemToLiAdapter,
    placeholderText = "Select an item",
    source,
    merger,
    getDisplay,
    getValue,
    onChange,
    value
}: Props<T, TValue>) {
    const [loading, setLoading] = useState(false);

    const rootRef = useRef<HTMLDivElement>(null);

    const [open, setOpen] = useState(false);

    const [state, setState] = useState<SelectState<T>>({
        items: [],
        nextToken: ''
    });

    const getMoreItems = useCallback(
        async (ignoreEmpty = true) => {
            setLoading(true);

            try {
                if (ignoreEmpty && state.nextToken === '') {
                    return;
                }

                const {
                    continuationToken,
                    items
                } = await source(state.nextToken);

                setState(old => ({
                    ...old,
                    items: merger(old.items, items),
                    nextToken: continuationToken
                }));
            } finally {
                setLoading(false);
            }
        },
        [state.nextToken, source, merger]
    );

    const onScroll: UIEventHandler<HTMLDivElement> = useCallback(
        event => {
            event.preventDefault();

            const div = event.currentTarget;

            const atBottom =
                div.scrollTop + div.clientHeight >= div.scrollHeight;

            if (atBottom && !loading) {
                getMoreItems();
            }
        },
        [loading, getMoreItems]
    );

    useEffect(() => {
        getMoreItems(false);
    }, []);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (
                rootRef.current &&
                !rootRef.current.contains(event.target as Node)
            ) {
                setOpen(false);
            }
        };

        document.addEventListener(
            "mousedown",
            handleClickOutside
        );

        return () => {
            document.removeEventListener(
                "mousedown",
                handleClickOutside
            );
        };
    }, []);

    const onClickSelect: MouseEventHandler<HTMLDivElement> = event => {
        event.preventDefault();

        setOpen(old => !old);
    };

    const onClickLi: MouseEventHandler<HTMLLIElement> = event => {
        const index = Number(
            event.currentTarget.dataset.index
        );

        if (
            Number.isNaN(index) ||
            index < 0 ||
            index >= state.items.length
        ) {
            return;
        }

        const item = state.items[index];

        onChange?.(getValue(item));

        setOpen(false);
    };

    const getPlaceholderText = () => {
        if (value == null) {
            return placeholderText;
        }

        const selectedItem = state.items.find(
            item => getValue(item) === value
        );

        if (!selectedItem) {
            return placeholderText;
        }

        return getDisplay(selectedItem);
    };

    return (
        <div
            ref={rootRef}
            className="flex flex-col gap-y-2"
        >
            <div
                className="cursor-pointer border rounded-md p-2 flex items-center justify-between"
                onClick={onClickSelect}
            >
                <span>{getPlaceholderText()}</span>

                <ArrowDown
                    className={`transition-all ${
                        open ? "rotate-180" : ""
                    }`}
                    size={16}
                    color="gray"
                />
            </div>

            <div
                className={`transition-all overflow-y-auto border rounded-md ${
                    open
                        ? "opacity-100 max-h-50"
                        : "opacity-0 max-h-0 border-transparent"
                }`}
                onScroll={onScroll}
            >
                <ul className="[&>li]:hover:bg-foreground/25 [&>li]:cursor-pointer [&>li]:p-2 [&>li.active]:bg-foreground/30">
                    {state.items.map((item, index) =>
                        itemToLiAdapter(
                            item,
                            onClickLi,
                            index,
                            value !== undefined &&
                                value === getValue(item)
                        )
                    )}

                    {loading && <Spinner />}
                </ul>
            </div>
        </div>
    );
}