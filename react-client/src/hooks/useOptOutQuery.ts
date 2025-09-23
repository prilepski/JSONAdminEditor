import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { optOutService } from '../services/optOutService';
import { OptOutLocale } from '../types';

export const useOptOutQuery = () => {
  return useQuery({
    queryKey: ['optOut'],
    queryFn: optOutService.getOptOutConfig,
    select: (data) => {
      // Convert nested data to table format
      const tableData: any[] = [];
      if (data && typeof data === 'object') {
        Object.entries(data).forEach(([lang, countries]) => {
          Object.entries(countries).forEach(([country, config]: [string, OptOutLocale]) => {
            tableData.push({
              'Language Code': lang,
              'Country Code': country,
              'Opt Out Keywords': config.optOutKeywords || '',
              'Opt In Keywords': config.optInKeywords || '',
              'Help Keywords': config.helpKeywords || '',
              'Opt Out Footer': config.optOutFooter || '',
              'Opt Out Phrase': config.optOutPhrase || '',
              'Opt In Phrase': config.optInPhrase || '',
              'Opt In Message': config.optInMessage || '',
              'Help Phrase': config.helpPhrase || ''
            });
          });
        });
      }
      
      return {
        columnNames: ['Language Code', 'Country Code', 'Opt Out Keywords', 'Opt In Keywords', 'Help Keywords', 'Opt Out Footer', 'Opt Out Phrase', 'Opt In Phrase', 'Opt In Message', 'Help Phrase'],
        columnTypes: { 
          'Language Code': 'text',
          'Country Code': 'text',
          'Opt Out Keywords': 'text', 
          'Opt In Keywords': 'text',
          'Help Keywords': 'text',
          'Opt Out Footer': 'text',
          'Opt Out Phrase': 'text',
          'Opt In Phrase': 'text',
          'Opt In Message': 'text',
          'Help Phrase': 'text'
        },
        tableData,
        filePath: 'opt-out-config',
        fileName: 'opt-out-config.json',
        isValidJson: true,
      };
    }
  });
};

export const useOptOutMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (tableData: any[]) => {
      // Convert table data back to nested structure
      const saveData: any = {};
      tableData.forEach(row => {
        const lang = row['Language Code'] as string || 'en';
        const country = row['Country Code'] as string || 'US';
        
        if (!saveData[lang]) saveData[lang] = {};
        
        saveData[lang][country] = {
          optOutKeywords: row['Opt Out Keywords'] as string || '',
          optInKeywords: row['Opt In Keywords'] as string || '',
          helpKeywords: row['Help Keywords'] as string || '',
          optOutFooter: row['Opt Out Footer'] as string || '',
          optOutPhrase: row['Opt Out Phrase'] as string || '',
          optInPhrase: row['Opt In Phrase'] as string || '',
          optInMessage: row['Opt In Message'] as string || '',
          helpPhrase: row['Help Phrase'] as string || ''
        };
      });
      
      return optOutService.saveOptOutConfig(saveData);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['optOut'] });
    },
  });
};