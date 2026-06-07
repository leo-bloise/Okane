import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import { RegisterFormSchema } from "@/lib/types/register";
import Link from "next/link";
import { FieldErrors, UseFormRegister } from "react-hook-form";

type Props = {
    onSubmit: () => void;
    register: UseFormRegister<RegisterFormSchema>;
    errors: FieldErrors<RegisterFormSchema>;
    loading: boolean;
}

export default function RegisterFormView({ errors, onSubmit, register, loading }: Props) {
    return <form onSubmit={onSubmit}>
        <FieldGroup>
            <Field>
                <FieldLabel>E-mail</FieldLabel>
                <Input type="email" placeholder="E-mail" {...register("email")} disabled={loading}/>
                <FieldError>{errors.email && errors.email.message}</FieldError>
            </Field>
            <Field>
                <FieldLabel>Password</FieldLabel>
                <Input type="password" placeholder="Password" {...register("password")} disabled={loading}/>
                <FieldError>{errors.password && errors.password.message}</FieldError>
            </Field>
            <Field>
                <FieldLabel>Confirm Password</FieldLabel>
                <Input type="password" placeholder="Confirm Password" {...register("confirmPassword")} disabled={loading}/>
                <FieldError>{errors.confirmPassword && errors.confirmPassword.message}</FieldError>
            </Field>
            <Field>
                <Button className="hover:cursor-pointer" type="submit" disabled={loading}>
                    {loading && <Spinner data-icon="inline-start"></Spinner>}
                    {loading ? "Loading" : "Register"}
                </Button>
            </Field>
            <Field className="-mt-3">
                <Link 
                    className="text-sm text-center text-black underline underline-offset-4"
                    href={"/login"}>Already have an account? Login in!</Link>
            </Field>
        </FieldGroup>
    </form>
}