import LoginForm from "@/components/custom/login-form";

export default function Home() {
  return <div className="flex flex-col justify-center h-screen items-center gap-y-8">
    <header className="w-1/4">
      <h1 className="text-xl font-bold">Okane</h1>
      <h3 className="text-sm">Simple as it needs to be</h3>
    </header>
    <main className="w-1/4 flex flex-col">
      <LoginForm></LoginForm>
    </main>
  </div>
}
