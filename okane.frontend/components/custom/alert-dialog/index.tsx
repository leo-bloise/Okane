import { Button } from "@/components/ui/button";

type Props = {
    
}

export default function AlertDialog() {
    return (
        <div className="fixed inset-0 z-50 bg-background/30 backdrop-blur-sm flex items-center justify-center">
            <div className="w-1/4 h-1/4 bg-background border rounded-md flex flex-col text-center p-4 justify-around">
                <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Dolore molestias temporibus similique iusto nihil. Eius aperiam molestias eos libero iusto ipsa ad, voluptas nisi nulla accusantium, veniam magni, autem obcaecati.</p>
                <Button className="cursor-pointer">Ok</Button>
            </div>
        </div>
    );
}