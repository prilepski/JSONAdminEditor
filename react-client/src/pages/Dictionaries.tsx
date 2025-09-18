import React, {useState} from 'react';
import {FileType, ValidationError, TableData} from '../types';
import {useDictionaryQuery, useDictionaryMutation, useUploadMutation} from '../hooks/useDictionaryQuery';
import {DictionarySelector} from '../components/DictionarySelector';
import {FileUpload} from '../components/FileUpload';
import {JsonEditor} from '../components/JsonEditor';
import toast from 'react-hot-toast';
import {Skeleton} from '../components/Skeleton';
import { PageHeader } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';

export const Dictionaries: React.FC = () => {
    const [selectedFileType, setSelectedFileType] = useState<FileType>(FileType.None);
  
    const [validationErrors, setValidationErrors] = useState<ValidationError[]>([]);

    const {data: dictionaryResponse, isLoading, error} = useDictionaryQuery(selectedFileType);
    const saveMutation = useDictionaryMutation();
    useUploadMutation();

    const dictionaryData = dictionaryResponse?.success ? dictionaryResponse.data : null;

    const handleFileTypeChange = (fileType: FileType) => {
        setSelectedFileType(fileType);
        setValidationErrors([]);
    };



    const handleSave = async (tableData: TableData[]) => {
        if (!dictionaryData) {
            return {success: false, error: 'No dictionary data available'};
        }

        try {
            const result = await saveMutation.mutateAsync({
                filePath: dictionaryData.filePath,
                jsonData: tableData,
                fileType: selectedFileType
            });

            if (result.success) {
                toast.success(result.message || 'Dictionary saved successfully!');
                return {success: true, message: result.message};
            } else {
                toast.error(result.error || 'Failed to save dictionary');
                if (result.validationErrors) {
                    setValidationErrors(result.validationErrors);
                }
                return {success: false, error: result.error};
            }
        } catch (error) {
            const errorMessage = 'Error saving dictionary';
            toast.error(errorMessage);
            return {success: false, error: errorMessage};
        }
    };

    return (
        <PageErrorBoundary pageName="Dictionaries">
            <PageHeader icon="fa-cog" title="Dictionary Management" />
            {error && toast.error(String(error))}

            <ComponentErrorBoundary componentName="Dictionary Selector">
                <DictionarySelector
                    selectedFileType={selectedFileType}
                    onFileTypeChange={handleFileTypeChange}
                />
            </ComponentErrorBoundary>

            {selectedFileType !== FileType.None && (
                <div className="card">
                    <div className="card-header">
                        <div className="row align-items-center">
                            <div className="col-md-6">
                                <h3>
                                    <i className="fas fa-edit me-2"></i>
                                    Dictionary Editor
                                </h3>
                            </div>
                            <div className="col-md-6">
                                <ComponentErrorBoundary componentName="File Upload">
                                    <FileUpload
                                        selectedFileType={selectedFileType}
                                        onUploadSuccess={(text) => toast.success(text)}
                                        onUploadError={(text) => toast.error(text)}
                                    />
                                </ComponentErrorBoundary>
                            </div>
                        </div>
                    </div>
                    <div className="card-body">
                        <Skeleton loading={isLoading} rows={5} height="40px">
                            <ComponentErrorBoundary componentName="JSON Editor">
                                <JsonEditor
                                    dictionaryData={dictionaryData}
                                    selectedFileType={selectedFileType}
                                    validationErrors={validationErrors}
                                    onSave={handleSave}
                                    onClearValidationErrors={() => setValidationErrors([])}
                                />
                            </ComponentErrorBoundary>
                        </Skeleton>
                    </div>
                </div>
            )}
        </PageErrorBoundary>
    );
};