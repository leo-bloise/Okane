import { ReactNode } from "react";
import Menu from "@/components/custom/menu";

export default function Layout({
    children,
}: Readonly<{
    children: ReactNode;
}>) {
    return <div className="min-h-screen grid grid-cols-12">
        <aside className="col-span-1 border-r p-4 flex flex-col">
            <header className="mb-10">
                <h1 className="text-xl font-bold">Okane</h1>
            </header>
            <Menu />
        </aside>
        <main className="col-span-11 w-full p-4">
            {children}
        </main>
    </div>
}