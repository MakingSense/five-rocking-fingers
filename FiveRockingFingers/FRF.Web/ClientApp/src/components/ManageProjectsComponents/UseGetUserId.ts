import React, { useState, useEffect } from 'react';
import axios from "axios";

interface IUser {
    userId: string;
    email: string;
}

function UseGetUserId(userEmail : any) {
    const [userId, setUserId] = useState<string | null>(null);

    useEffect(() => {
        async function fetchUserId() {
            const response = await axios.post("https://localhost:44346/api/Projects/GetUserId/" + userEmail);
            if (response.status === 200) {
                setUserId(response.data);
            }
        }
        fetchUserId();
    },[]);
    console.log(userId);
    return userId;
}
export default UseGetUserId;