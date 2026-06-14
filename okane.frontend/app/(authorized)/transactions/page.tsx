import CreateTransactionForm from "@/components/custom/create-transaction-form";
import TransactionsSection from "@/components/custom/transactions";
import { Drawer, DrawerContent, DrawerDescription, DrawerHeader, DrawerTitle, DrawerTrigger } from "@/components/ui/drawer";
import { fetchFromServer } from "@/lib/api.server-component";
import { Paged } from "@/lib/types/paged";
import { Transaction } from "@/lib/types/transaction";
import { Plus } from "lucide-react";

async function fetchTransactions(page: number, pageSize: number) {
    const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString()
    });

    const response = await fetchFromServer(
        `/transactions/api?${params}`
    );

    if (!response.ok) {
        throw new Error('Failed to fetch transactions');
    }

    return response.json() as Promise<Paged<Transaction>>
}

type PageProps = {
    searchParams: Promise<{
        page?: string;
        pageSize?: string;
    }>
}

export default async function Page({ searchParams }: PageProps) {
    const params = await searchParams;

    const page = Number(params.page ?? 0);
    const pageSize = Number(params.pageSize ?? 20);

    const { details } = await fetchTransactions(page, pageSize);

    const transactions = details!.items
    const totalPages = details!.totalPages

    return <div className="min-h-full flex flex-col gap-y-5">
        <header className="flex items-center">
            <h2 className="text-xl font-bold">Transactions</h2>
            <Drawer direction="right">
                <DrawerTrigger className="hover:cursor-pointer ml-2 bg-primary text-secondary rounded-md p-1 right-10 absolute">
                    <Plus />
                </DrawerTrigger>
                <DrawerContent>
                    <DrawerHeader>
                        <DrawerTitle>Register Transaction</DrawerTitle>
                        <DrawerDescription>Register previously occured transactions</DrawerDescription>
                    </DrawerHeader>
                    <div className="p-4">
                        <CreateTransactionForm />
                    </div>
                </DrawerContent>
            </Drawer>
        </header>
        <TransactionsSection
            transactions={transactions}
            currentPage={page}
            totalPages={totalPages}
            pageSize={pageSize}
        />
        {transactions.length == 0 && <section className="flex flex-col items-center">
            <h2 className="text-md">No transactions created</h2>
        </section>}
    </div>;
}