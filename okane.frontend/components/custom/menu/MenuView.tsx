'use client';

import Link from "next/link";
import { usePathname } from "next/navigation";
import { ReactNode } from "react";

type Props = {
    items: {
        icon: ReactNode,
        text: string,
        url: string
    }[]
}

export default function MenuView({ items }: Props) {
    const pathname = usePathname();

    return <nav>
        <ul className="flex flex-col gap-y-6">
            {items.map(item =>
                <li key={item.text}>
                    <Link className="flex flex-row gap-x-2 items-center"  href={item.url}>
                        {item.icon}
                        <span className="text-sm hidden lg:block">{item.text}</span>
                    </Link>
                </li>
            )}
        </ul>
    </nav>
}