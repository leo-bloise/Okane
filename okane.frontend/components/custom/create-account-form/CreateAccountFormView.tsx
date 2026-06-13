import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import { Textarea } from "@/components/ui/textarea";
import { CreateAccountFormSchema } from "@/lib/types/accounts";
import { FieldErrors, UseFormRegister } from "react-hook-form";

type Props = {
    onSubmit: () => Promise<any>;
    register: UseFormRegister<CreateAccountFormSchema>;
    errors: FieldErrors<CreateAccountFormSchema>;
    loading: boolean;
}

export default function CreateAccountFormView({
    errors,
    loading,
    onSubmit,
    register
}: Props) { 
    return <form 
        className="flex flex-col gap-y-4" 
        onSubmit={onSubmit}
    >                    
        <Field>
            <FieldLabel>Name</FieldLabel>
            <Input disabled={loading} type="text" placeholder="Checking Account" {...register('name')}></Input>
            <FieldError>{errors.name && errors.name.message}</FieldError>
        </Field>
        <Field>
            <FieldLabel>Description</FieldLabel>
            <Textarea disabled={loading} placeholder="XPTO Bank holding most of my finances." {...register('description')}/>
            <FieldError>{errors.description && errors.description.message}</FieldError>
        </Field>
        <Field>
            <Button className="hover:cursor-pointer" type="submit" disabled={loading}>
                {loading && <Spinner data-icon="inline-start"></Spinner>}
                {loading ? "Loading" : "Create"}
            </Button>
        </Field>
    </form>
}