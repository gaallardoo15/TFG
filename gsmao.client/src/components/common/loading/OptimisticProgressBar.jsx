import {useEffect, useState} from "react";
import {ProgressBar} from "react-bootstrap";

export const OptimisticProgressBar = ({isLoading, estimatedDuration, isUploadComplete}) => {
    const [progress, setProgress] = useState(0);

    useEffect(() => {
        let interval = null;
        const maxProgress = 99.5;
        const startTime = Date.now();

        if (isLoading) {
            setProgress(0);

            interval = setInterval(() => {
                const elapsedTime = Date.now() - startTime;
                const timeFraction = elapsedTime / estimatedDuration;
                const newProgress = maxProgress * (1 - Math.exp(-3 * timeFraction));

                setProgress(() => {
                    if (isUploadComplete) {
                        if (interval) {
                            clearInterval(interval);
                        }
                        return 100;
                    } else {
                        return Math.min(newProgress, maxProgress);
                    }
                });
            }, 200);
        } else {
            setProgress(0);
        }

        return () => {
            if (interval) {
                clearInterval(interval);
            }
        };
    }, [isLoading, estimatedDuration, isUploadComplete]);

    return <ProgressBar now={progress} animated label={`${Math.round(progress)}%`} className="custom-progress-bar" />;
};
