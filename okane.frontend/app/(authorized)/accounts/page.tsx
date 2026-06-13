import AccountsSection from "@/components/custom/accounts";
import CreateAccountForm from "@/components/custom/create-account-form";
import { Drawer, DrawerContent, DrawerDescription, DrawerHeader, DrawerTitle, DrawerTrigger } from "@/components/ui/drawer";
import { fetchFromServer } from "@/lib/api.server-component";
import { AccountResponse } from "@/lib/service/accounts.service";
import { Plus } from "lucide-react";

async function fetchAccounts(page: number, pageSize: number) {
    const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
    });

    const response = await fetchFromServer(
        `/accounts/api?${params}`
    );

    if (!response.ok) {
        throw new Error("Failed to fetch accounts");
    }

    return response.json() as Promise<AccountResponse>;
}

type PageProps = {
    searchParams: Promise<{
        page?: string;
        pageSize?: string;
    }>;
};

export default async function Page({ searchParams }: PageProps) {
    const params = await searchParams;

    const page = Number(params.page ?? 0);
    const pageSize = Number(params.pageSize ?? 20);

    const { details } = await fetchAccounts(page, pageSize)

    const accounts = details!.items;
    const totalPages = details!.totalPages;

    return (
        <div className="min-h-full flex flex-col gap-y-5">
            <header className="flex items-center">
                <h2 className="text-xl font-bold">Accounts</h2>
                <Drawer  direction="right">
                    <DrawerTrigger className="hover:cursor-pointer ml-2 bg-primary text-secondary rounded-md p-1 right-10 absolute">
                        <Plus />
                    </DrawerTrigger>
                    <DrawerContent>
                        <DrawerHeader>
                            <DrawerTitle>Create Account</DrawerTitle>
                            <DrawerDescription>Create an account to register your transactions</DrawerDescription>
                        </DrawerHeader>
                        <div className="p-4">
                            <CreateAccountForm />
                        </div>
                    </DrawerContent>
                </Drawer>
            </header>
            <AccountsSection
                accounts={accounts}
                currentPage={page}
                totalPages={totalPages}
                pageSize={pageSize}
            />
            {accounts.length == 0 && <section className="flex flex-col items-center">
                <h2 className="text-md">No accounts created</h2>
            </section>}
        </div>
    );
}