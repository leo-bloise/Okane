import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import Link from "next/link";

export default function LoginFormView() {
    return <form>
        <FieldGroup>
            <Field>
                <FieldLabel>E-mail</FieldLabel>
                <Input type="email" placeholder="E-mail" />
                <FieldError></FieldError>
            </Field>
            <Field>
                <FieldLabel>Password</FieldLabel>
                <Input type="password" placeholder="Password" />
                <FieldError />
            </Field>
            <Field>
                <Button type="submit">Login</Button>
            </Field>
            <Field className="-mt-3">
                <Link 
                    className="text-sm text-center text-black underline underline-offset-4"
                    href={"/register"}>Don't have an account? Create one here!</Link>
            </Field>
        </FieldGroup>
    </form>
}