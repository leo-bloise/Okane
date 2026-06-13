import { Button } from "@/components/ui/button";
import { Field } from "@/components/ui/field";
import { Input } from "@/components/ui/input";

type Props = {
    onSearch: (query: string) => void;
}

export default function AccountsSearchHandler({ onSearch }: Props) {
    return <Field orientation={"horizontal"}>
        <Input type="search" placeholder="Search for accounts" onChange={(event) => {
            event.preventDefault();
            onSearch(event.target.value)
        }}></Input>
        <Button className="cursor-pointer">Search</Button>
    </Field>
}