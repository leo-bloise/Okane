'use client';

import { ArrowDown } from "lucide-react";
import { MouseEventHandler, ReactNode, UIEventHandler, useCallback, useEffect, useState } from "react";
import { Spinner } from "./spinner";

type SelectState<T> = {
    items: T[];
    selectedItemIndex: number;
    getSelectedItem: () => T | undefined;
    nextToken: string;
}

type Props<T> = {
    itemToLiAdapter: (data: T, onClickListener: MouseEventHandler<HTMLLIElement>, index: number, selectedIndex: number) => ReactNode;
    placeholderText?: string;
    source: (continuationToken: string) => Promise<{ items: T[], continuationToken: string }>;
    getDisplay(value: T): string;
    merger: (oldItems: T[], newItems: T[]) => T[];
}

export default function InifiniteSelect<T>({ itemToLiAdapter, placeholderText = "Select an item", source, merger, getDisplay }: Props<T>) {
    const [loading, setLoading] = useState<boolean>(false);

    const [state, setState] = useState<SelectState<T>>({
        items: [] as T[],
        selectedItemIndex: -1,
        getSelectedItem(this: {
            items: T[],
            selectedItemIndex: number
        }) {
            if (this.selectedItemIndex == -1) return undefined;

            return this.items[this.selectedItemIndex] as T;
        },
        nextToken: ''
    });

    const getMoreItems = useCallback(async (ignoreEmpty: boolean = true) => {
        setLoading(true);
        try {
            if(ignoreEmpty && state.nextToken === '') return;

            const { continuationToken, items } = await source(state.nextToken);

            setState(old => ({
                ...old,
                items: merger(old.items, items),
                nextToken: continuationToken
            }));
        } finally {
            setLoading(false);
        }
    }, [state.nextToken]);

    const onScroll: UIEventHandler<HTMLDivElement> = useCallback((event) => {
        event.preventDefault();
        event.stopPropagation();

        const div = (event.target as HTMLDivElement);

        const atBottom = div.scrollTop + div.clientHeight >= div.scrollHeight;

        if(atBottom && !loading) {
            getMoreItems();
        }
    }, [loading]);

    useEffect(() => {
        getMoreItems(false);
    }, []);

    const onClickSelect: MouseEventHandler<HTMLDivElement> = (event) => {
        event.preventDefault();
        event.stopPropagation();

        const parent = (event.target as HTMLDivElement).parentElement as HTMLDivElement;

        if (parent.classList.contains('open')) {
            parent.classList.remove('open');
            return;
        }

        parent.classList.add('open');
    };

    const onClickLi: MouseEventHandler<HTMLLIElement> = (event) => {
        const index = event.currentTarget.dataset.index;

        if(index === undefined || index === null) {
            throw new Error('attach the index desired to the option.')
        }

        const i = Number(index);

        if(i < 0 || i >= state.items.length || Number.isNaN(i) || !Number.isInteger(i)) {
            throw new Error(`Invalid index selected. It must be between 0 and ${state.items.length}`)
        }
        
        setState(old => ({
            ...old,
            selectedItemIndex: i
        }));
    };

    const getPlaceholderText = useCallback(() => {
        const selectedItem = state.getSelectedItem();

        if(selectedItem === undefined) {
            return placeholderText;
        }

        return getDisplay(selectedItem);
    }, [state.selectedItemIndex, getDisplay]);

    return <div className="group flex flex-col gap-y-2">
        <div className="cursor-pointer h-1/12 max-h-50 border rounded-md overflow-y-auto p-2 flex items-center justify-between" onClick={onClickSelect}>
            <span>{getPlaceholderText()}</span>
            <ArrowDown className="group-[.open]:rotate-180 transition-all" size='16' color="gray" />
        </div>
        <div className="transition-all max-h-0 opacity-0 group-[.open]:opacity-100 group-[.open]:max-h-50 group-[.open]:h-1/12 border rounded-md overflow-y-auto" onScroll={onScroll}>
            <ul className="[&>li]:hover:bg-foreground/25 [&>li]:cursor-pointer [&>li]:p-2 [&>li.active]:bg-foreground">
                {state.items.map((item, index) => itemToLiAdapter(item, onClickLi, index, state.selectedItemIndex))}
                {loading && <Spinner />}
            </ul>
        </div>
    </div>
}